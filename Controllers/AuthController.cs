using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Zion.Reminder.Api.Utils;

namespace Zion.Reminder.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("token")]
        public IActionResult GetToken()
        {
            // In a real scenario, validate the requesting service here
            var secret = _configuration["Jwt:Secret"] ?? "zion-reminder-project-strong-secret-token";
            var token = JwtTokenGenerator.GenerateToken(secret);
            return Ok(new { token });
        }
    }
}
