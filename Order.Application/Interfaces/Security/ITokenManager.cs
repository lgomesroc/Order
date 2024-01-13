using Order.Application.DataContract.Response.User;
using Order.Domain.Models;

namespace Order.Application.Interfaces.Security
{
    public interface ITokenManager
    {
        Task<AuthResponse> GenerateTokenAsync(UserModel user);
    }
}
