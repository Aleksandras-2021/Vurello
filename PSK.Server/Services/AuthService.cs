using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using PSK.Server.Data.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PSK.Server.Services;

public interface IAuthService
{
    Task<string?> RegisterAsync(Register model);
    Task<string?> LoginAsync(Login model);
    Task<string?> RefreshAsync(string token);
}

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;

    public AuthService(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<string?> RegisterAsync(Register model)
    {
        var user = new User
        {
            UserName = model.Username,
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
            return null;

        return GenerateJwtToken(user);
    }

    public async Task<string?> LoginAsync(Login model)
    {
        var user = await _userManager.FindByNameAsync(model.Username);

        if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
            return null;

        return GenerateJwtToken(user);
    }

    public async Task<string?> RefreshAsync(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

        if (jwtToken == null)
            return null;

        var expirationDate = jwtToken.ValidTo;
        var currentDate = DateTime.UtcNow;

        if (expirationDate - currentDate > TimeSpan.FromHours(1))
            return token;

        var userId = jwtToken.Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
            return null;

        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
            return null;

        return GenerateJwtToken(user);
    }

    private string GenerateJwtToken(User user)
    {
        var claims = new[]
        {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.UserName),
        new Claim("userId", user.Id.ToString()),
    };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes("SecretSecret123...SecretAAAAAAAAAAA"));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            "Issuer",
            "Audience",
            claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

}
