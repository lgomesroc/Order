using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Order.Interfaces.Applications;
using Order.Request;
using System.Security.Claims;

namespace Order.Controllers
{
    [Route("api/user")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserApplication _UserApplication;

        public UserController(IUserApplication UserApplication)
        {
            _UserApplication = UserApplication;
        }

        [HttpGet]
        public async Task<ActionResult<List<Product>>> Get()
        {
            var products = await _UserApplication.ListAsync();
            var result = products;

            return Ok(result);
        }

        private ActionResult UnprocessableEntity(object report)
        {
            throw new NotImplementedException();
        }

        // GET api/<UserController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<UserController>
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Post([FromBody] CreateUserRequest request)
        {
            var response = await _UserApplication.CreateAsync(request);

            if (response.Report.Any())
                return UnprocessableEntity(response.Report);

            return Ok(response);
        }

        // PUT api/<UserController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<UserController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        // POST api/auth
        [HttpPost("auth")]
        [AllowAnonymous]
        public async Task<ActionResult> Auth([FromBody] AuthRequest request)
        {
            var claimsPrincipal = User.Identity as ClaimsPrincipal;
            var name = claimsPrincipal?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            await _UserApplication.ListByFilterAsync(claimsPrincipal, name);

            return Ok();
        }
    }
}
