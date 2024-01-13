using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Order.Interfaces.Applications;
using Order.Request;
using Order.Requests;

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

        // GET: api/<ClientController>
        /// <summary>
        /// Get all clients 
        /// </summary>
        /// <param name="clientid"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> Get([FromQuery] string clientid, [FromQuery] string name)
        {
            var response = await _clientApplication.ListByFilterAsync(clientid, name);

            if (response.Any()) // Verifica se há itens na lista
                return UnprocessableEntity(response);

            return Ok(response);
        }


        // GET api/<ClientController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult> Get(string id)
        {
            var response = await _clientApplication.GetByIdAsync(id);

            if (response.Report.Any())
                return UnprocessableEntity(response.Report);

            return Ok(response);
        }

        // POST api/<ClientController>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] CreateClientRequest request)
        {
            var response = await _clientApplication.CreateAsync(request);

            if (response.Any(item => item.Report.Any()))
                return UnprocessableEntity(response);

            return Ok(response);
        }


        // PUT api/<ClientController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] UpdateClientRequest request)
        {
            var updateResult = await _clientApplication.UpdateAsync(request);

            if (updateResult.Report.Any())
                return UnprocessableEntity(updateResult.Report);

            return Ok(updateResult);
        }


        // DELETE api/<ClientController>/5
        [HttpDelete("{id}")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            await _clientApplication.DeleteAsync(id);

            return Ok(); 
        }


    }
}