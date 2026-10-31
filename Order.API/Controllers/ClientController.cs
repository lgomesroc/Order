using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Order.Application.DataContract.Request.Client;
using Order.Application.Interfaces;

namespace Order.Controllers
{
    [Route("api/client")]
    [ApiController]
    [Authorize]
    public class ClientController : ControllerBase
    {
        private readonly IClientApplication _clientApplication;

        public ClientController(IClientApplication clientApplication)
        {
            _clientApplication = clientApplication;
        }

        /// <summary>Lists clients, optionally filtered by id or name.</summary>
        [HttpGet]
        public async Task<ActionResult> Get([FromQuery] string clientId, [FromQuery] string name)
        {
            var response = await _clientApplication.ListByFilterAsync(clientId, name);

            if (response.Report.Any())
                return UnprocessableEntity(response.Report);

            return Ok(response.Data);
        }

        /// <summary>Gets a client by id.</summary>
        [HttpGet("{id}")]
        public async Task<ActionResult> GetById(string id)
        {
            var response = await _clientApplication.GetByIdAsync(id);

            if (response.Report.Any())
                return UnprocessableEntity(response.Report);

            return Ok(response.Data);
        }

        /// <summary>Creates a client.</summary>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] CreateClientRequest request)
        {
            var response = await _clientApplication.CreateAsync(request);

            if (response.Report.Any())
                return UnprocessableEntity(response.Report);

            return StatusCode(StatusCodes.Status201Created);
        }

        /// <summary>Updates a client.</summary>
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(string id, [FromBody] UpdateClientRequest request)
        {
            request.Id = id;

            var response = await _clientApplication.UpdateAsync(request);

            if (response.Report.Any())
                return UnprocessableEntity(response.Report);

            return NoContent();
        }

        /// <summary>Deletes a client.</summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            var response = await _clientApplication.DeleteAsync(id);

            if (response.Report.Any())
                return UnprocessableEntity(response.Report);

            return NoContent();
        }
    }
}
