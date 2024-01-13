using Order.Request;
using System.Security.Claims;

namespace Order.Interfaces.Applications
{
    public interface IProductApplication
    {
        Task AuthAsync(AuthRequest request);
        Task<CreateUserResponse> CreateAsync(CreateUserRequest request);
        Task<CreateProductResponse> CreateAsync(CreateProductRequest request);
        Task<List<Product>> ListByFilterAsync(string productId, string name);
        Task<List<User>> ListUsersByFilterAsync(ClaimsPrincipal product, string name);
        Task<List<User>> ListUsersByFilterAsync(ClaimsPrincipal product, object name);
        //Task <List<Product>> ListByFilterAsync(string productId, string description);
    }

    public class CreateUserResponse
    {
        public object Report { get; set; }
        public string ProductId { get; set; }
        public string Name { get; set; }
        public bool Success { get; set; }
    }

    public class CreateProductResponse
    {
    }

    public class CreateClientResponse
    {
    }
    
}