// Built with help from Claude
using System.Security.Claims;

namespace AgriTrack.Api.Extensions;

public static class ClaimsExtensions
{
    // Reads the user id from the JWT (the same claim we stored in TokenService)
    public static string GetUserId(this ClaimsPrincipal user)
    {
        return user.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new InvalidOperationException("User id missing from token");
    }
}