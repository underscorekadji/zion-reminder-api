using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Zion.Reminder.Api.Utils;
using Zion.Reminder.Config;

namespace Zion.Reminder.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly JwtSettings jwtSettings;

        public AuthController(IOptions<JwtSettings> jwtOptions)
        {
            jwtSettings = jwtOptions.Value;
        }

        [HttpPost("token")]
        public IActionResult GetToken()
        {
            // В реальном сценарии здесь должна быть валидация сервиса
            if (string.IsNullOrWhiteSpace(jwtSettings.Secret))
                throw new InvalidOperationException("JWT secret is not configured. Please set Jwt__Secret in appsettings or environment variables.");
            var token = JwtTokenGenerator.GenerateToken(jwtSettings.Secret);
            return Ok(new { token });
        }
    }
}
