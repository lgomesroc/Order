using Order.Request;
using Order.Requests;
using Order.Responses;

namespace Order.Interfaces.Applications
{
    public interface IClientApplication
    {
        //Task CreateAsync(CreateClientRequest request);
        //Task UpdateAsync(UpdateClientRequest request);
        Task DeleteAsync(string id);
        Task<ClientResponse> GetByIdAsync(string id);
        Task<List<ClientResponse>> ListByFilterAsync(string clientid, string name);
        Task<List<ClientResponse>> CreateAsync(CreateClientRequest request);
        Task<ClientUpdateResponse> UpdateAsync(UpdateClientRequest request);

        Task CreateAsync(List<CreateClientRequest> requests);
        Task DeleteAsync(List<string> ids);
    }
}