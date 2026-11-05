// Built with help from Claude
using System.ComponentModel.DataAnnotations;

namespace AgriTrack.Api.Dtos;

public record RegisterRequest(
    [Required] string FullName,
    [Required, EmailAddress] string Email,
    [Required] string Password);

public record LoginRequest(
    [Required, EmailAddress] string Email,
    [Required] string Password);

public record AuthResponse(string Token, DateTime ExpiresAt, string Email, string FullName);