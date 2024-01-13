using Order.Domain.Models;
using Order.Domain.Validations.Base;

namespace Order.Domain.Interfaces.Services
{
    public interface IClientService
    {
        Task<Response> CreateAsync(ClientModel client);
        Task<Response> UpdateAsync(ClientModel client);
        Task<Response> DeleteAsync(string clientId);
        Task<Response<ClientModel>> GetByIdAsync(string clientId);
        Task<Response<List<ClientModel>>> ListByFiltersAsync(string clientId = null, string name = null);
    }
}
