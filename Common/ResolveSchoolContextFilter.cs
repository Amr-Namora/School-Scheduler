using Microsoft.AspNetCore.Mvc.Filters;
using SchoolScheduler.Application.Common.Interfaces;
using System.Threading.Tasks;

namespace Common;

public class ResolveSchoolContextFilter : IAsyncActionFilter
{
    private readonly ICurrentSchoolContext _schoolContext;

    public ResolveSchoolContextFilter(ICurrentSchoolContext schoolContext)
    {
        _schoolContext = schoolContext;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // Automatically resolve the SchoolId before the controller action runs,
        // but ONLY if the user is logged in as a School.
        if (context.HttpContext.User.Identity?.IsAuthenticated == true &&
            context.HttpContext.User.IsInRole("School"))
        {
            await _schoolContext.GetSchoolIdAsync();
        }

        // Continue to the controller
        await next();
    }
}