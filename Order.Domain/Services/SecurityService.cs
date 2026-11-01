using System.Threading.Tasks;
using Order.Domain.Interfaces.Services;
using Order.Domain.Models;
using Order.Domain.Validations.Base;

namespace Order.Domain.Services
{
    public class SecurityService : ISecurityService
    {
        public Task<Response<bool>> ComparePassword(string password, string confirmPassword)
        {
            var isEquals = !string.IsNullOrWhiteSpace(password)
                && !string.IsNullOrWhiteSpace(confirmPassword)
                && string.Equals(password, confirmPassword, StringComparison.Ordinal);

            return Task.FromResult(Response.OK<bool>(isEquals));
        }

        public Task<Response<string>> EncryptPassword(string password)
        {
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

            return Task.FromResult(Response.OK<string>(passwordHash));
        }

        public Task<Response<bool>> VerifyPassword(string password, UserModel user)
        {
            bool validPassword = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);

            return Task.FromResult(Response.OK<bool>(validPassword));
        }
    }
}