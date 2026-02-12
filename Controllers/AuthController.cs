using EMS.Api.Data;
using EMS.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace EMS.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _configuration;

    public AuthController(AppDbContext db, IConfiguration configuration)
    {
        _db = db;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(User user)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var existingUser = await _db.Users.FirstOrDefaultAsync(u => u.Username == user.Username);

        if (existingUser != null)
        {
            return BadRequest("Username already exists.");
        }
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return Ok("User registered successfully.");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(User login)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == login.Username);

        if (user == null)
        {
            return Unauthorized("Invalid credentials.");
        }
        if (!BCrypt.Net.BCrypt.Verify(login.PasswordHash, user.PasswordHash))
        {
            return Unauthorized("Invalid credentials.");
        }
        string token = GenerateJwtToken(user);
        return Ok(new { token });
    }

    private string GenerateJwtToken(User user)
    {
        var jwtKey = _configuration["Jwt:Key"];
        var jwtIssuer = _configuration["Jwt:Issuer"];
        if (string.IsNullOrEmpty(jwtKey))
        {
            throw new InvalidOperationException("JWT Key is not configured.");
        }
        var securityKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(securityKey, Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, user.Username),
            new System.Security.Claims.Claim("UserId", user.Id.ToString())
        };

        var token = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(
            issuer: jwtIssuer,
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(2),
            signingCredentials: credentials
        );

        return new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().WriteToken(token);
    }

}


