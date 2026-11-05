// Built with help from Claude
using Microsoft.AspNetCore.Identity;

namespace AgriTrack.Api.Models;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = "";
}