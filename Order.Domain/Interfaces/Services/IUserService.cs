using Order.Domain.Models;
using Order.Domain.Validations.Base;

namespace Order.Domain.Interfaces.Services
{
    public interface IUserService
    {
        Task<Response<bool>> AutheticationAsync(string password, UserModel user);
        Task<Response> CreateAsync(UserModel user);
        Task<Response> UpdateAsync(UserModel user);
        Task<Response> DeleteAsync(string userId);
        Task<Response<UserModel>> GetByIdAsync(string userId);
        Task<Response<UserModel>> GetByLoginAsync(string login);
        Task<Response<List<UserModel>>> ListByFilterAsync(string userId = null, string name = null);
    }
}
