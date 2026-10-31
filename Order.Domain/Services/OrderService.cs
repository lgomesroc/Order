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

        public OrderService(ITimeProvider timeProvider,
                            IGenerators generators,
                            IUnitOfWork unitOfWork)
        {
            _timeProvider = timeProvider;
            _generators = generators;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Creates an order. Everything happens in a single transaction: the order and its items
        /// are saved and the stock of every product is decreased, or nothing is saved at all.
        /// </summary>
        public async Task<Response> CreateAsync(OrderModel order)
        {
            var errors = new OrderValidation().Validate(order).GetErrors();

            if (errors.Report.Count > 0)
                return errors;

            var response = new Response();

            _unitOfWork.BeginTransaction();

            try
            {
                if (!await _unitOfWork.ClientRepository.ExistsByIdAsync(order.Client.Id))
                    response.Report.Add(Report.Create($"Client {order.Client.Id} not exists!"));

                if (!await _unitOfWork.UserRepository.ExistsByIdAsync(order.User.Id))
                    response.Report.Add(Report.Create($"User {order.User.Id} not exists!"));

                if (response.Report.Count > 0)
                {
                    _unitOfWork.RollbackTransaction();
                    return response;
                }

                var now = _timeProvider.utcDateTime();

                order.Id = _generators.Generate();
                order.CreatedAt = now;

                // The same product may appear in more than one item: keep one instance per product
                // so the stock is decreased cumulatively.
                var products = new Dictionary<string, ProductModel>();

                foreach (var item in order.Items)
                {
                    var productId = item.Product.Id;

                    if (!products.TryGetValue(productId, out var product))
                    {
                        product = await _unitOfWork.ProductRepository.GetByIdAsync(productId);

                        if (product == null)
                        {
                            response.Report.Add(Report.Create($"Product {productId} not exists!"));
                            continue;
                        }

                        products[productId] = product;
                    }

                    if (product.Stock < item.Quantity)
                    {
                        response.Report.Add(Report.Create(
                            $"Insufficient stock for product {productId}: requested {item.Quantity}, available {product.Stock}."));
                        continue;
                    }

                    product.Stock -= item.Quantity;

                    item.Id = _generators.Generate();
                    item.CreatedAt = now;
                    item.Order = order;
                    item.Product = product;

                    if (item.SellValue <= 0)
                        item.SellValue = product.SellValue;

                    item.TotalAmount = item.SellValue * item.Quantity;
                }

                if (response.Report.Count > 0)
                {
                    _unitOfWork.RollbackTransaction();
                    return response;
                }

                await _unitOfWork.OrderRepository.CreateAsync(order);

                foreach (var product in products.Values)
                    await _unitOfWork.ProductRepository.UpdateAsync(product);

                _unitOfWork.CommitTransaction();

                return response;
            }
            catch
            {
                _unitOfWork.RollbackTransaction();
                throw;
            }
        }

        public async Task<Response> DeleteAsync(string orderId)
        {
            var response = new Response();

            if (!await _unitOfWork.OrderRepository.ExistsByIdAsync(orderId))
            {
                response.Report.Add(Report.Create($"Order {orderId} not exists!"));
                return response;
            }

            _unitOfWork.BeginTransaction();

            try
            {
                await _unitOfWork.OrderRepository.DeleteAsync(orderId);
                _unitOfWork.CommitTransaction();

                return response;
            }
            catch
            {
                _unitOfWork.RollbackTransaction();
                throw;
            }
        }

        public async Task<Response<OrderModel>> GetByIdAsync(string orderId)
        {
            var response = new Response<OrderModel>();

            if (!await _unitOfWork.OrderRepository.ExistsByIdAsync(orderId))
            {
                response.Report.Add(Report.Create($"Order {orderId} not exists!"));
                return response;
            }

            var data = await _unitOfWork.OrderRepository.GetByIdAsync(orderId);
            data.Items = await _unitOfWork.OrderRepository.ListItemByOrderIdAsync(orderId);

            response.Data = data;
            return response;
        }

        public async Task<Response<List<OrderModel>>> ListByFiltersAsync(string orderId = null, string clientId = null, string userId = null)
        {
            var response = new Response<List<OrderModel>>();

            if (!string.IsNullOrWhiteSpace(orderId) && !await _unitOfWork.OrderRepository.ExistsByIdAsync(orderId))
            {
                response.Report.Add(Report.Create($"Order {orderId} not exists!"));
                return response;
            }

            response.Data = await _unitOfWork.OrderRepository.ListByFilterAsync(orderId, clientId, userId);
            return response;
        }

        public async Task<Response> UpdateAsync(OrderModel order)
        {
            var errors = new OrderValidation().Validate(order).GetErrors();

            if (errors.Report.Count > 0)
                return errors;

            var response = new Response();

            if (!await _unitOfWork.OrderRepository.ExistsByIdAsync(order.Id))
            {
                response.Report.Add(Report.Create($"Order {order.Id} not exists!"));
                return response;
            }

            _unitOfWork.BeginTransaction();

            try
            {
                await _unitOfWork.OrderRepository.UpdateAsync(order);
                _unitOfWork.CommitTransaction();

                return response;
            }
            catch
            {
                _unitOfWork.RollbackTransaction();
                throw;
            }
        }
    }
}
