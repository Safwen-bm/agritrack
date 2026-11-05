// Built with help from Claude
using System.Security.Claims;
using AgriTrack.Api.Dtos;
using AgriTrack.Api.Models;
using AgriTrack.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AgriTrack.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _users;
    private readonly TokenService _tokens;

    public AuthController(UserManager<ApplicationUser> users, TokenService tokens)
    {
        _users = users;
        _tokens = tokens;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
    {
        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FullName = request.FullName
        };

        // Identity hashes the password for us
        var result = await _users.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors.Select(e => e.Description));
        }

        return Ok(_tokens.CreateToken(user));
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        var user = await _users.FindByEmailAsync(request.Email);

        // Same message for both cases, so attackers can't tell which emails exist
        if (user == null || !await _users.CheckPasswordAsync(user, request.Password))
        {
            return Unauthorized(new { message = "Invalid email or password" });
        }

        return Ok(_tokens.CreateToken(user));
    }

    // Protected endpoint: only works with a valid token
    [Authorize]
    [HttpGet("me")]
    public ActionResult Me()
    {
        return Ok(new
        {
            id = User.FindFirstValue(ClaimTypes.NameIdentifier),
            email = User.FindFirstValue(ClaimTypes.Email),
            name = User.FindFirstValue(ClaimTypes.Name)
        });
    }
}