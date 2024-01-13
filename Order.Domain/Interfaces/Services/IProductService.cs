using Order.Domain.Models;
using Order.Domain.Validations.Base;

namespace Order.Domain.Interfaces.Services
{
    public interface IProductService
    {
        Task<Response> CreateAsync(ProductModel product);
        Task<Response> UpdateAsync(ProductModel product);
        Task<Response> DeleteAsync(string productId);
        Task<Response<ProductModel>> GetByIdAsync(string productId);
        Task<Response<List<ProductModel>>> ListByFilterAsync(string productId = null, string description = null);
    }
}
