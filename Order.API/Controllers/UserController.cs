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
        private readonly IUserApplication _userApplication;

        public UserController(IUserApplication userApplication)
        {
            _userApplication = userApplication;
        }

        /// <summary>Lists users, optionally filtered by id or name.</summary>
        [HttpGet]
        public async Task<ActionResult> Get([FromQuery] string userId, [FromQuery] string name)
        {
            var response = await _userApplication.ListByFilterAsync(userId, name);

            if (response.Report.Any())
                return UnprocessableEntity(response.Report);

            return Ok(response.Data);
        }

        /// <summary>Registers a new user. Open to anonymous callers.</summary>
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Post([FromBody] CreateUserRequest request)
        {
            var response = await _userApplication.CreateAsync(request);

            if (response.Report.Any())
                return UnprocessableEntity(response.Report);

            return StatusCode(StatusCodes.Status201Created);
        }

        /// <summary>Authenticates a user and returns a JWT.</summary>
        [HttpPost("auth")]
        [AllowAnonymous]
        public async Task<ActionResult> Auth([FromBody] AuthRequest request)
        {
            var response = await _userApplication.AuthAsync(request);

            if (response.Report.Any())
                return Unauthorized(response.Report);

            return Ok(response.Data);
        }
    }
}
