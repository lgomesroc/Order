using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Order.Interfaces.Applications;
using Order.Request;

namespace Order.Controllers
{
    [Route("api/product")]
    [ApiController]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly IProductApplication _ProductApplication;

        public ProductController(IProductApplication ProductApplication)
        {
            _ProductApplication = ProductApplication;
        }

        /// <summary>
        /// Get all products
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        public async Task<ActionResult> Get([FromQuery] string productId, [FromQuery] string description)
        {
            var response = await _ProductApplication.ListByFilterAsync(productId, description);

            if (response.Any())
                return UnprocessableEntity(response);

            return Ok(response);
        }

        // GET api/<ProductController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<ProductController>
        [HttpPost]
        public async Task<ActionResult<CreateProductResponse>> Post([FromBody] CreateProductRequest request)
        {
            var response = await _ProductApplication.CreateAsync(request);

            if (response != null && !string.IsNullOrEmpty(response.ToString()))
                return UnprocessableEntity(response.ToString());

            return Ok(response);
        }

        // PUT api/<ProductController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ProductController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
