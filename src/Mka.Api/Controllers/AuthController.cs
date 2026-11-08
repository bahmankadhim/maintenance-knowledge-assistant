using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mka.Api.Auth;

namespace Mka.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(TokenService tokens, IConfiguration config, IWebHostEnvironment env) : ControllerBase
{
    [HttpPost("login")]
    [AllowAnonymous]
    public ActionResult<TokenResponse> Login(LoginRequest request)
    {
        if(!env.IsDevelopment()) return NotFound();

        var users = config.GetSection("DemoUsers").Get<List<DemoUser>>() ?? [];
        var user = users.FirstOrDefault(u => u.Username.Equals(request.Username, StringComparison.OrdinalIgnoreCase) && u.Password == request.Password);

        if(user is null) return Unauthorized();

        return Ok(tokens.CreateToken(user.Username, user.Role));
    }
}