using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Order.Application.DataContract.Request.User;
using Order.Application.Interfaces;

namespace Order.Controllers
{
    [Route("api/user")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserApplication _application;
        private readonly IConfiguration _configuration;

        public UserController(IUserApplication application, IConfiguration configuration)
        {
            _application = application;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string userId, [FromQuery] string name)
        {
            var response = await _application.ListByFilterAsync(userId, name);
            if (response.Report.Any()) return UnprocessableEntity(response.Report);
            return Ok(response.Data);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Post([FromBody] CreateUserRequest request)
        {
            if (!_configuration.GetValue<bool>("Registration:PublicEnabled"))
                return NotFound();

            var response = await _application.CreateAsync(request);
            if (response.Report.Any()) return UnprocessableEntity(response.Report);
            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpPost("auth")]
        [AllowAnonymous]
        public async Task<IActionResult> Auth([FromBody] AuthRequest request)
        {
            var response = await _application.AuthAsync(request);
            if (response.Report.Any()) return Unauthorized(response.Report);
            return Ok(response.Data);
        }
    }
}
