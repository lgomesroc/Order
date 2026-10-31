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
        private readonly IProductApplication _productApplication;

        public ProductController(IProductApplication productApplication)
        {
            _productApplication = productApplication;
        }

        /// <summary>Lists products, optionally filtered by id or description.</summary>
        [HttpGet]
        public async Task<ActionResult> Get([FromQuery] string productId, [FromQuery] string description)
        {
            var response = await _productApplication.ListByFilterAsync(productId, description);

            if (response.Report.Any())
                return UnprocessableEntity(response.Report);

            return Ok(response.Data);
        }

        /// <summary>Creates a product.</summary>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] CreateProductRequest request)
        {
            var response = await _productApplication.CreateAsync(request);

            if (response.Report.Any())
                return UnprocessableEntity(response.Report);

            return StatusCode(StatusCodes.Status201Created);
        }
    }
}
