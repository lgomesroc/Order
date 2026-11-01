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
            var result = await _application.ListByFilterAsync(productId, description);
            return result.Report.Any() ? UnprocessableEntity(result.Report) : Ok(result.Data);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _application.GetByIdAsync(id);
            return result.Report.Any() ? NotFound(result.Report) : Ok(result.Data);
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateProductRequest request)
        {
            var result = await _application.CreateAsync(request);
            return result.Report.Any() ? UnprocessableEntity(result.Report) : StatusCode(StatusCodes.Status201Created);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody] UpdateProductRequest request)
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
