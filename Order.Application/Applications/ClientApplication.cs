using AutoMapper;
using Order.Application.DataContract.Request.Client;
using Order.Application.DataContract.Response.Client;
using Order.Application.Interfaces;
using Order.Domain.Interfaces.Services;
using Order.Domain.Models;
using Order.Domain.Validations.Base;

namespace Order.Application.Applications
{
    public class ClientApplication : IClientApplication
    {
        private readonly IClientService _clientService;
        private readonly IMapper _mapper;

        public ClientApplication(IClientService clientService, IMapper mapper)
        {
            _clientService = clientService;
            _mapper = mapper;
        }

        public async Task<Response> CreateAsync(CreateClientRequest client)
        {
            try
            {
                var clientModel = _mapper.Map<ClientModel>(client);

                return await _clientService.CreateAsync(clientModel);
            }
            catch (Exception ex)
            {
                var response = Report.Create(ex.Message);

                return Response.Unprocessable(response);
            }
        }

        public async Task<Response> DeleteAsync(string clientId)
        {
            return await _clientService.DeleteAsync(clientId);
        }

        // Tipos de retorno corrigidos
        public async Task<Response<clientResponse>> GetByIdAsync(string clientId)
        {
            Response<ClientModel> client = await _clientService.GetByIdAsync(clientId);

            if (client.Report.Any())
                return Response.Unprocessable<clientResponse>(client.Report);

            var response = _mapper.Map<clientResponse>(client.Data);

            return Response.OK(response);
        }

        public async Task<Response<List<clientResponse>>> ListByFilterAsync(string clientId, string name)
        {
            Response<List<ClientModel>> client = await _clientService.ListByFiltersAsync(clientId, name);

            if (client.Report.Any())
                return Response.Unprocessable<List<clientResponse>>(client.Report);

            var response = _mapper.Map<List<clientResponse>>(client.Data);

            return Response.OK(response);
        }

        public async Task<Response> UpdateAsync(UpdateClientRequest request)
        {
            var clientModel = _mapper.Map<ClientModel>(request);

            return await _clientService.UpdateAsync(clientModel);
        }

        // Métodos duplicados removidos
    }
}
