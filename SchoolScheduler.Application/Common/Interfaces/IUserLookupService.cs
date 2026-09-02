using System.Threading.Tasks;

namespace SchoolScheduler.Application.Common.Interfaces;

public interface IUserLookupService
{
    Task<string?> FindUserIdByEmailAsync(string email);
    Task<string?> GetFullNameAsync(string userId);
}
