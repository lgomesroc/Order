using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Order.Application.DataContract.Request.Client;
using Order.Application.Interfaces;

namespace Order.Controllers
{
    [Route("api/clients")]
    [ApiController]
    [Authorize]
    public class ClientController : ControllerBase
    {
        private readonly IClientApplication _application;
        public ClientController(IClientApplication application) => _application = application;

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string clientId, [FromQuery] string name)
        {
            var response = await _application.ListByFilterAsync(clientId, name);
            if (response.Report.Any()) return UnprocessableEntity(response.Report);
            return Ok(response.Data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var response = await _application.GetByIdAsync(id);
            if (response.Report.Any()) return NotFound(response.Report);
            return Ok(response.Data);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateClientRequest request)
        {
            var response = await _application.CreateAsync(request);
            if (response.Report.Any()) return UnprocessableEntity(response.Report);
            return CreatedAtAction(nameof(GetById), new { id = "" }, null);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody] UpdateClientRequest request)
        {
            request.Id = id;
            var response = await _application.UpdateAsync(request);
            if (response.Report.Any()) return UnprocessableEntity(response.Report);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var response = await _application.DeleteAsync(id);
            if (response.Report.Any()) return UnprocessableEntity(response.Report);
            return NoContent();
        }
    }
}
