using Order.Application.DataContract.Request.Order;
using Order.Domain.Validations.Base;

namespace Order.Application.Interfaces
{
    public interface IOrderApplication
    {
        Task<Response> CreateAsync(CreateOrderRequest Order);
        Task<Response<List<OrderResponse>>> ListByFilterAsync(string orderId = null, string clientId = null, string userId = null);
        Task<Response<OrderResponse>> GetByIdAsync(string orderId);
    }
}
