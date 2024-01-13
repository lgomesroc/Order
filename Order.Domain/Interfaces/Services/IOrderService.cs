using Order.Domain.Models;
using Order.Domain.Validations.Base;

namespace Order.Domain.Interfaces.Services
{
    public interface IOrderService
    {
        Task<Response> CreateAsync(OrderModel order);
        Task<Response> UpdateAsync(OrderModel order);
        Task<Response> DeleteAsync(OrderModel order);
        Task<Response<OrderModel>> GetByIdAsync(string orderID);
        Task<Response<List<OrderModel>>> ListByFiltersAsync(string orderId = null, string clientId = null, string userId = null);
    }
}
