using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Order.Application.DataContract.Request.Product;
using Order.Application.Interfaces;

namespace Order.Controllers
{
    [Route("api/product")]
    [ApiController]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly IProductApplication _application;
        public ProductController(IProductApplication application) => _application = application;

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string productId, [FromQuery] string description)
        {
            var response = await _application.ListByFilterAsync(productId, description);
            if (response.Report.Any()) return UnprocessableEntity(response.Report);
            return Ok(response.Data);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateProductRequest request)
        {
            var response = await _application.CreateAsync(request);
            if (response.Report.Any()) return UnprocessableEntity(response.Report);
            return StatusCode(StatusCodes.Status201Created);
        }
    }
}
