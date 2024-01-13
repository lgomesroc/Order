using Order.Application.DataContract.Request.Product;
using Order.Application.DataContract.Response.Product;
using Order.Domain.Validations.Base;

namespace Order.Application.Interfaces
{
    public interface IProductApplication
    {
        Task<Response> CreateAsync(CreateProductRequest Product);
        Task<Response<List<ProductResponse>>> ListByFilterAsync(string productId = null, string description = null);
    }
}
