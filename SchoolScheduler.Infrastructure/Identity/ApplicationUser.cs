using Microsoft.AspNetCore.Identity;

namespace SchoolScheduler.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
}
