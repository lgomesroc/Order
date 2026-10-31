using System.Security.Claims;
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
        private readonly IOrderApplication _orderApplication;

        public OrderController(IOrderApplication orderApplication)
        {
            _orderApplication = orderApplication;
        }

        /// <summary>Lists orders, optionally filtered by order, client or user.</summary>
        [HttpGet]
        public async Task<ActionResult> Get([FromQuery] string orderId, [FromQuery] string clientId, [FromQuery] string userId)
        {
            var response = await _orderApplication.ListByFilterAsync(orderId, clientId, userId);

            if (response.Report.Any())
                return UnprocessableEntity(response.Report);

            return Ok(response.Data);
        }

        /// <summary>Gets an order with its items.</summary>
        [HttpGet("{id}")]
        public async Task<ActionResult> GetById(string id)
        {
            var response = await _orderApplication.GetByIdAsync(id);

            if (response.Report.Any())
                return UnprocessableEntity(response.Report);

            return Ok(response.Data);
        }

        /// <summary>
        /// Creates an order for the authenticated user. The stock of each product is decreased
        /// and the order is rejected as a whole when any item cannot be fulfilled.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] CreateOrderRequest request)
        {
            // The seller is always the authenticated user, never a value sent by the client.
            request.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var response = await _orderApplication.CreateAsync(request);

            if (response.Report.Any())
                return UnprocessableEntity(response.Report);

            return StatusCode(StatusCodes.Status201Created);
        }

        /// <summary>Deletes an order and its items.</summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            var response = await _orderApplication.DeleteAsync(id);

            if (response.Report.Any())
                return UnprocessableEntity(response.Report);

            return NoContent();
        }
    }
}
