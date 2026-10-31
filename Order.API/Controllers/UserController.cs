using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Order.Application.DataContract.Request.User;
using Order.Application.Interfaces;

namespace Order.Controllers
{
    [Route("api/users")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserApplication _application;
        public UserController(IUserApplication application) => _application = application;

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
