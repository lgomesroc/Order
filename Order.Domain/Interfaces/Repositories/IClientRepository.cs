using Order.Domain.Models;

namespace Order.Domain.Interfaces.Repositories
{
    public interface IClientRepository
    {
        Task CreateAsync(ClientModel client);
        Task UpdateAsync(ClientModel client);
        Task DeleteAsync(ClientModel client);
        Task<bool> ExistsByIdAsync(string clientId);
        Task<ClientModel> GetByIdAsync(string clientId);
        Task<List<ClientModel>> ListByFilterAsync(string clientId = null, string name = null);
        Task DeleteAsync(string clientId);
    }
}
