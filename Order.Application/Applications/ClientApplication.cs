using Order.Application.DataContract.Request.Client;
using Order.Application.DataContract.Response.Client;
using Order.Application.Interfaces;
using Order.Application.Mapper;
using Order.Domain.Interfaces.Services;
using Order.Domain.Validations.Base;

namespace Order.Application.Applications;

public class ClientApplication(IClientService clientService) : IClientApplication
{
    public async Task<Response> CreateAsync(CreateClientRequest request)
    {
        try { return await clientService.CreateAsync(ApplicationMapper.ToModel(request)); }
        catch (Exception ex) { return Response.Unprocessable(Report.Create(ex.Message)); }
    }

    public Task<Response> DeleteAsync(string clientId) => clientService.DeleteAsync(clientId);

    public async Task<Response<clientResponse>> GetByIdAsync(string clientId)
    {
        var result = await clientService.GetByIdAsync(clientId);
        return result.Report.Any()
            ? Response.Unprocessable<clientResponse>(result.Report)
            : Response.OK(ApplicationMapper.ToResponse(result.Data));
    }

    public async Task<Response<List<clientResponse>>> ListByFilterAsync(string clientId, string name)
    {
        var result = await clientService.ListByFiltersAsync(clientId, name);
        return result.Report.Any()
            ? Response.Unprocessable<List<clientResponse>>(result.Report)
            : Response.OK(result.Data.Select(ApplicationMapper.ToResponse).ToList());
    }

    public Task<Response> UpdateAsync(UpdateClientRequest request) =>
        clientService.UpdateAsync(ApplicationMapper.ToModel(request));
}
