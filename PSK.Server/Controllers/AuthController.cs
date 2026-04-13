using Microsoft.AspNetCore.Mvc;
using PSK.Server.Data.Entities;
using PSK.Server.Services;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(Register model)
    {
        var token = await _authService.RegisterAsync(model);

        if (token == null)
            return BadRequest("Registration failed");

        return Ok(new { Token = token });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(Login model)
    {
        var token = await _authService.LoginAsync(model);

        if (token == null)
            return Unauthorized();

        return Ok(new { Token = token });
    }

    [HttpPost("refresh/{token}")]
    public async Task<IActionResult> Refresh(string token)
    {
        var newToken = await _authService.RefreshAsync(token);

        if (newToken == null)
            return Unauthorized();

        return Ok(new { Token = newToken });
    }
}