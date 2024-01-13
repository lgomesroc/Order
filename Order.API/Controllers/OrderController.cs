using Microsoft.AspNetCore.Mvc;
using Order.Interfaces.Applications;
using Order.Request;

namespace Order.Controllers
{
    [Route("api/order")]
    [ApiController]
    //[Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderApplication _OrderApplication;

        public OrderController(IOrderApplication OrderApplication)
        {
            _OrderApplication = OrderApplication;
        }

        [HttpGet]
        public async Task<ActionResult> Get([FromQuery] string orderId, [FromQuery] string clientId, [FromQuery] string userId)
        {
            var response = await _OrderApplication.ListByFilterAsync(orderId, clientId, userId);

            if (response.Any())
                return UnprocessableEntity(response);

            return Ok(response);
        }

        // GET api/<OrderController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult> GetById(string id)
        {
            var response = await _OrderApplication.GetByIdAsync(id);

            if (response.Report.Any())
                return UnprocessableEntity(response.Report);

            return Ok(response);
        }

        // POST api/<OrderController>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] CreateOrderRequest request)
        {
            var response = await _OrderApplication.CreateAsync(request);

            if (response.Report.Any())
                return UnprocessableEntity(response.Report);

            return Ok(response);
        }

        // PUT api/<OrderController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<OrderController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}