using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Order.Application.DataContract.Request.Order;
using Order.Application.Interfaces;
namespace Order.Controllers
{
    [Route("api/order")]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderApplication _application;
        public OrderController(IOrderApplication application) => _application = application;
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string orderId, [FromQuery] string clientId, [FromQuery] string userId)
        {
            var result = await _application.ListByFilterAsync(orderId, clientId, userId);
            return result.Report.Any() ? UnprocessableEntity(result.Report) : Ok(result.Data);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _application.GetByIdAsync(id);
            return result.Report.Any() ? NotFound(result.Report) : Ok(result.Data);
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateOrderRequest request)
        {
            var result = await _application.CreateAsync(request);
            return result.Report.Any() ? UnprocessableEntity(result.Report) : StatusCode(StatusCodes.Status201Created);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody] UpdateOrderRequest request)
        {
            request.Id = id;
            var result = await _application.UpdateAsync(request);
            return result.Report.Any() ? UnprocessableEntity(result.Report) : NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _application.DeleteAsync(id);
            return result.Report.Any() ? NotFound(result.Report) : NoContent();
        }
    }
}
