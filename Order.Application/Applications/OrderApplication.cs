using AutoMapper;
using Order.Application.DataContract.Request.Order;
using Order.Application.DataContract.Response.Order;
using Order.Application.Interfaces;
using Order.Domain.Interfaces.Services;
using Order.Domain.Models;
using Order.Domain.Validations.Base;

namespace Order.Application.Applications
{
    public class OrderApplication : IOrderApplication
    {
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;

        public OrderApplication(IOrderService orderService, IMapper mapper)
        {
            _orderService = orderService;
            _mapper = mapper;
        }

        public async Task<Response> CreateAsync(CreateOrderRequest order)
        {
            var orderModel = _mapper.Map<OrderModel>(order);

            return await _orderService.CreateAsync(orderModel);
        }

        public async Task<Response> DeleteAsync(string orderId)
        {
            return await _orderService.DeleteAsync(orderId);
        }

        public async Task<Response<OrderResponse>> GetByIdAsync(string orderId)
        {
            Response<OrderModel> order = await _orderService.GetByIdAsync(orderId);

            if (order.Report.Any())
                return Response.Unprocessable<OrderResponse>(order.Report);

            return Response.OK(_mapper.Map<OrderResponse>(order.Data));
        }

        public async Task<Response<List<OrderResponse>>> ListByFilterAsync(string orderId = null, string clientId = null, string userId = null)
        {
            Response<List<OrderModel>> orders = await _orderService.ListByFiltersAsync(orderId, clientId, userId);

            if (orders.Report.Any())
                return Response.Unprocessable<List<OrderResponse>>(orders.Report);

            return Response.OK(_mapper.Map<List<OrderResponse>>(orders.Data));
        }
    }
}
