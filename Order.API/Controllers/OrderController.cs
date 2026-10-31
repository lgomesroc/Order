using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Order.Application.DataContract.Request.Order;
using Order.Application.Interfaces;

namespace Order.Controllers
{
    [Route("api/orders")]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderApplication _application;
        public OrderController(IOrderApplication application) => _application = application;

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string orderId, [FromQuery] string clientId, [FromQuery] string userId)
        {
            var response = await _application.ListByFilterAsync(orderId, clientId, userId);
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
        public async Task<IActionResult> Post([FromBody] CreateOrderRequest request)
        {
            var response = await _application.CreateAsync(request);
            if (response.Report.Any()) return UnprocessableEntity(response.Report);
            return StatusCode(StatusCodes.Status201Created);
        }
    }
}
