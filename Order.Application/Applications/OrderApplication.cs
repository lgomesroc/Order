using AutoMapper;
using Order.Application.DataContract.Request.Order;
using Order.Application.Interfaces;
using Order.Domain.Interfaces.Services;
using Order.Domain.Models;
using Order.Domain.Validations.Base;
using OrderResponse = Order.Application.DataContract.Response.Order.OrderResponse;

namespace Order.Application.Applications
{
    public class OrderApplication : IOrderApplication
    {
        private readonly IOrderService _OrderService;
        private readonly IMapper _mapper;

        public OrderApplication(IOrderService OrderService, IMapper mapper)
        {
            _OrderService = OrderService;
            _mapper = mapper;
        }

        public async Task<Response> CreateAsync(CreateOrderRequest Order)
        {
            try
            {
                var OrderModel = _mapper.Map<OrderModel>(Order);

                return await _OrderService.CreateAsync(OrderModel);
            }
            catch (Exception ex)
            {
                var response = Report.Create(ex.Message);

                return Response.Unprocessable(response);
            }
        }

        public async Task<Response<OrderResponse>> GetByIdAsync(string orderId)
        {
            Response<OrderModel> user = await _OrderService.GetByIdAsync(orderId);

            if (user.Report.Any())
                return Response.Unprocessable<OrderResponse>(user.Report);

            var response = _mapper.Map<OrderResponse>(user.Data);

            return Response.OK(response);
        }

        public async Task<Response<List<OrderResponse>>> ListByFilterAsync(string orderId = null, string clientId = null, string userId = null)
        {
            Response<List<OrderModel>> user = await _OrderService.ListByFiltersAsync(orderId, clientId, userId);

            if (user.Report.Any())
                return Response.Unprocessable<List<OrderResponse>>(user.Report);

            var response = _mapper.Map<List<OrderResponse>>(user.Data);

            return Response.OK(response);
        }

        Task<Response> IOrderApplication.CreateAsync(CreateOrderRequest Order)
        {
            throw new NotImplementedException();
        }

        Task<Response<Interfaces.OrderResponse>> IOrderApplication.GetByIdAsync(string orderId)
        {
            throw new NotImplementedException();
        }

        Task<Response<List<Interfaces.OrderResponse>>> IOrderApplication.ListByFilterAsync(string orderId, string clientId, string userId)
        {
            throw new NotImplementedException();
        }

        // Métodos duplicados removidos
    }
}
