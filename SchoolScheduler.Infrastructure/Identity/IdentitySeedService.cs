using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace SchoolScheduler.Infrastructure.Identity;

public class IdentitySeedService
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ILogger<IdentitySeedService> _logger;

    public IdentitySeedService(RoleManager<IdentityRole> roleManager, ILogger<IdentitySeedService> logger)
    {
        _roleManager = roleManager;
        _logger = logger;
    }

    public async Task SeedRolesAsync()
    {
        var roles = new[] { "School", "Teacher" };

        foreach (var roleName in roles)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                _logger.LogInformation("Seeding role: {RoleName}", roleName);
                await _roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
    }
}
