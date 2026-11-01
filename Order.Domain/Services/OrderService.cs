using Order.Domain.Common;
using Order.Domain.Interfaces.Repositories;
using Order.Domain.Interfaces.Services;
using Order.Domain.Models;
using Order.Domain.Validations;
using Order.Domain.Validations.Base;

namespace Order.Domain.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITimeProvider _timeProvider;
        private readonly IGenerators _generators;

        public OrderService(ITimeProvider timeProvider, IGenerators generators, IUnitOfWork unitOfWork)
        {
            _timeProvider = timeProvider;
            _generators = generators;
            _unitOfWork = unitOfWork;
        }

        public async Task<Response> CreateAsync(OrderModel order)
        {
            var response = Validate(order);
            if (response.Report.Any()) return response;

            _unitOfWork.BeginTransaction();
            try
            {
                if (!await RelatedEntitiesExistAsync(order, response))
                {
                    _unitOfWork.RollbackTransaction();
                    return response;
                }

                order.Id = _generators.Generate();
                order.CreatedAt = _timeProvider.utcDateTime();

                if (!await ReserveItemsAsync(order, response))
                {
                    _unitOfWork.RollbackTransaction();
                    return response;
                }

                await _unitOfWork.OrderRepository.CreateAsync(order);
                _unitOfWork.CommitTransaction();
                return response;
            }
            catch
            {
                _unitOfWork.RollbackTransaction();
                response.Report.Add(Report.Create("The order could not be created. No changes were saved."));
                return response;
            }
        }

        public async Task<Response> UpdateAsync(OrderModel order)
        {
            var response = Validate(order);
            if (response.Report.Any()) return response;

            _unitOfWork.BeginTransaction();
            try
            {
                var current = await _unitOfWork.OrderRepository.GetByIdAsync(order.Id);
                if (current == null)
                {
                    _unitOfWork.RollbackTransaction();
                    response.Report.Add(Report.Create($"Order {order.Id} does not exist."));
                    return response;
                }

                if (!await RelatedEntitiesExistAsync(order, response))
                {
                    _unitOfWork.RollbackTransaction();
                    return response;
                }

                current.Items = await _unitOfWork.OrderRepository.ListItemByOrderIdAsync(order.Id);
                await RestoreItemsAsync(current.Items);

                if (!await ReserveItemsAsync(order, response))
                {
                    _unitOfWork.RollbackTransaction();
                    return response;
                }

                await _unitOfWork.OrderRepository.UpdateAsync(order);
                _unitOfWork.CommitTransaction();
                return response;
            }
            catch
            {
                _unitOfWork.RollbackTransaction();
                response.Report.Add(Report.Create("The order could not be updated. No changes were saved."));
                return response;
            }
        }

        public async Task<Response> DeleteAsync(string orderId)
        {
            var response = new Response();
            _unitOfWork.BeginTransaction();
            try
            {
                var exists = await _unitOfWork.OrderRepository.ExistsByIdAsync(orderId);
                if (!exists)
                {
                    _unitOfWork.RollbackTransaction();
                    response.Report.Add(Report.Create($"Order {orderId} does not exist."));
                    return response;
                }

                var items = await _unitOfWork.OrderRepository.ListItemByOrderIdAsync(orderId);
                await RestoreItemsAsync(items);
                await _unitOfWork.OrderRepository.DeleteAsync(orderId);
                _unitOfWork.CommitTransaction();
                return response;
            }
            catch
            {
                _unitOfWork.RollbackTransaction();
                response.Report.Add(Report.Create("The order could not be deleted. No changes were saved."));
                return response;
            }
        }

        public async Task<Response<OrderModel>> GetByIdAsync(string orderId)
        {
            var response = new Response<OrderModel>();
            _unitOfWork.BeginTransaction();
            try
            {
                var data = await _unitOfWork.OrderRepository.GetByIdAsync(orderId);
                if (data == null)
                {
                    _unitOfWork.RollbackTransaction();
                    response.Report.Add(Report.Create($"Order {orderId} does not exist."));
                    return response;
                }

                data.Items = await _unitOfWork.OrderRepository.ListItemByOrderIdAsync(orderId);
                _unitOfWork.CommitTransaction();
                response.Data = data;
                return response;
            }
            catch
            {
                _unitOfWork.RollbackTransaction();
                response.Report.Add(Report.Create("The order could not be retrieved."));
                return response;
            }
        }

        public async Task<Response<List<OrderModel>>> ListByFiltersAsync(string orderId = null, string clientId = null, string userId = null)
        {
            var response = new Response<List<OrderModel>>();
            var data = await _unitOfWork.OrderRepository.ListByFilterAsync(orderId, clientId, userId);
            response.Data = data;
            return response;
        }

        private static Response Validate(OrderModel order)
        {
            var validation = new OrderValidation();
            return validation.Validate(order).GetErrors();
        }

        private async Task<bool> RelatedEntitiesExistAsync(OrderModel order, Response response)
        {
            if (!await _unitOfWork.ClientRepository.ExistsByIdAsync(order.Client.Id))
            {
                response.Report.Add(Report.Create($"Client {order.Client.Id} does not exist."));
                return false;
            }

            if (!await _unitOfWork.UserRepository.ExistsByIdAsync(order.User.Id))
            {
                response.Report.Add(Report.Create($"User {order.User.Id} does not exist."));
                return false;
            }

            return true;
        }

        private async Task<bool> ReserveItemsAsync(OrderModel order, Response response)
        {
            foreach (var item in order.Items)
            {
                if (item.Product == null || string.IsNullOrWhiteSpace(item.Product.Id))
                {
                    response.Report.Add(Report.Create("Every order item must reference a product."));
                    return false;
                }

                var product = await _unitOfWork.ProductRepository.GetByIdAsync(item.Product.Id);
                if (product == null)
                {
                    response.Report.Add(Report.Create($"Product {item.Product.Id} does not exist."));
                    return false;
                }

                var stockReserved = await _unitOfWork.ProductRepository.TryDecreaseStockAsync(product.Id, item.Quantity);
                if (!stockReserved)
                {
                    response.Report.Add(Report.Create($"Insufficient stock for product {product.Id}."));
                    return false;
                }

                product.Stock -= item.Quantity;

                item.Product = product;
                item.SellValue = product.SellValue;
                item.TotalAmount = product.SellValue * item.Quantity;
                item.Order = order;
                item.Id = _generators.Generate();
                item.CreatedAt = _timeProvider.utcDateTime();
            }

            return true;
        }

        private async Task RestoreItemsAsync(IEnumerable<OrderItemModel> items)
        {
            foreach (var item in items)
            {
                if (item.Product == null || string.IsNullOrWhiteSpace(item.Product.Id)) continue;
                await _unitOfWork.ProductRepository.IncreaseStockAsync(item.Product.Id, item.Quantity);
            }
        }
    }
}
