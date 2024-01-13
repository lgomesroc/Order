using Order.Request;

namespace Order.Extensions
{
    public interface IUserApplication
    {
        Task AuthAsync(AuthRequest request);
    }
}