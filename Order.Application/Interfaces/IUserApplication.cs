using Order.Application.DataContract.Request.User;
using Order.Application.DataContract.Response.User;
using Order.Domain.Validations.Base;

namespace Order.Application.Interfaces
{
    public interface IUserApplication
    {
        Task<Response<AuthResponse>> AuthAsync(AuthRequest auth);
        Task<Response> CreateAsync(CreateUserRequest User);
        Task<Response<List<UserResponse>>> ListByFilterAsync(string userId = null, string name = null);
    }
}
