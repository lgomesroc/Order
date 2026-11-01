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
            if (!result.Report.Any()) return Ok(result.Data);
            return result.Report.Any(report => report.Code == "NOT_FOUND")
                ? NotFound(result.Report)
                : UnprocessableEntity(result.Report);
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
            if (!result.Report.Any()) return NoContent();
            return result.Report.Any(report => report.Code == "NOT_FOUND")
                ? NotFound(result.Report)
                : UnprocessableEntity(result.Report);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _application.DeleteAsync(id);
            if (!result.Report.Any()) return NoContent();
            return result.Report.Any(report => report.Code == "NOT_FOUND")
                ? NotFound(result.Report)
                : UnprocessableEntity(result.Report);
        }
    }
}
