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
        private readonly IOrderService _service;
        private readonly IMapper _mapper;
        public OrderApplication(IOrderService service, IMapper mapper) { _service = service; _mapper = mapper; }
        public Task<Response> CreateAsync(CreateOrderRequest request) => _service.CreateAsync(_mapper.Map<OrderModel>(request));
        public Task<Response> UpdateAsync(UpdateOrderRequest request) => _service.UpdateAsync(_mapper.Map<OrderModel>(request));
        public Task<Response> DeleteAsync(string id) => _service.DeleteAsync(id);
        public async Task<Response<OrderResponse>> GetByIdAsync(string id)
        {
            var result = await _service.GetByIdAsync(id);
            return result.Report.Any() ? Response.Unprocessable<OrderResponse>(result.Report) : Response.OK(_mapper.Map<OrderResponse>(result.Data));
        }
        public async Task<Response<List<OrderResponse>>> ListByFilterAsync(string orderId = null, string clientId = null, string userId = null)
        {
            var result = await _service.ListByFiltersAsync(orderId, clientId, userId);
            return result.Report.Any() ? Response.Unprocessable<List<OrderResponse>>(result.Report) : Response.OK(_mapper.Map<List<OrderResponse>>(result.Data));
        }
    }
}
