using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Data;
using Library.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TokenController : ControllerBase {
    private readonly IConfiguration _config;
    private readonly DataContext _context;

    public TokenController(IConfiguration config, DataContext context) {
        _config = config;
        _context = context;
    }

    public async Task<IActionResult> Post([FromForm]User userData) {
        if (userData is not { Login: not null, Password: not null }) return BadRequest();
        
        var user = await GetFullUserInfo(userData.Login, userData.Password);

        if (user is null) return BadRequest("Invalid credentials");

        var role = Role.Student.ToString();

        if (user.Role is Role.Manager) {
            var manager = await _context.Managers!.FirstAsync(m => m.UserId == user.Id);
            role = Role.Manager + manager.Role.ToString();
        }
        
        var claims = new[] {
            new Claim(JwtRegisteredClaimNames.Sub, _config["Jwt:Subject"]!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
            new Claim(ClaimsIdentity.DefaultRoleClaimType, role),
            new Claim("UserId", user.Id.ToString()),
            new Claim("Login", user.Login),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            _config["Jwt:Issuer"],
            _config["Jwt:Audience"],
            claims,
            expires: DateTime.UtcNow.AddHours(12),
            signingCredentials: signIn
        );

        return Ok(new JwtSecurityTokenHandler().WriteToken(token));
    }
    
    private async Task<User?> GetFullUserInfo(string login, string password) {
        return await _context.Users?.FirstOrDefaultAsync(u => u.Login == login && u.Password == password);
    }
}