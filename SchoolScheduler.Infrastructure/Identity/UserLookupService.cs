using Microsoft.AspNetCore.Identity;
using SchoolScheduler.Application.Common.Interfaces;
using SchoolScheduler.Infrastructure.Identity;
using System.Threading.Tasks;

namespace SchoolScheduler.Infrastructure.Identity;

public class UserLookupService : IUserLookupService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserLookupService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<string?> FindUserIdByEmailAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        return user?.Id;
    }

    public async Task<string?> GetFullNameAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        return user?.FullName;
    }
}
