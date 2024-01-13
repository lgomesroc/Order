using System.Security.Claims;
using Order.Request;

namespace Order.Interfaces.Applications
{
    public interface IUserApplication
    {
        Task AuthAsync(AuthRequest request);
        Task<CreateUserResponse> CreateAsync(CreateUserRequest request);
        Task<List<User>> ListAsync();
        Task<List<User>> ListByFilterAsync(string userId, string name);
        Task ListByFilterAsync(ClaimsPrincipal user, object name);
        Task ListByFilterAsync(ClaimsPrincipal user, string name);
        Task ListByFilterAsync(ClaimsPrincipal claimsPrincipal);

        public class CreateUserResponse
        {
            public List<ReportItem> Report { get; set; }

            public string UserId { get; set; }
            public string Name { get; set; }
            public bool Success { get; set; }
        }
    }
}