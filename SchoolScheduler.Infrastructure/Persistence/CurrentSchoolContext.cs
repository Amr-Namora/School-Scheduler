using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SchoolScheduler.Application.Common.Interfaces;
using System;
using System.Threading.Tasks;

namespace SchoolScheduler.Infrastructure.Persistence;

public class CurrentSchoolContext : ICurrentSchoolContext
{
    private readonly ICurrentUserContext _currentUserContext;
    private readonly IServiceProvider _serviceProvider;
    private Guid? _cachedSchoolId;
    private bool _isResolved = false;

    public CurrentSchoolContext(ICurrentUserContext currentUserContext, IServiceProvider serviceProvider)
    {
        _currentUserContext = currentUserContext;
        _serviceProvider = serviceProvider;
    }

    public Guid? SchoolId
    {
        get
        {
            if (!_isResolved)
            {
                throw new InvalidOperationException("School context has not been resolved. Call GetSchoolIdAsync() first.");
            }
            return _cachedSchoolId;
        }
    }

    public async Task<Guid?> GetSchoolIdAsync()
    {
        if (_isResolved) return _cachedSchoolId;

        var userId = _currentUserContext.UserId;

        // 1. Check if user is logged in FIRST. 
        if (string.IsNullOrEmpty(userId))
        {
            _cachedSchoolId = null;
            _isResolved = true;
            return null;
        }

        // 2. Role Validation
        if (_currentUserContext.Role != "School")
        {
            throw new UnauthorizedAccessException("School context is only available to users with the 'School' role.");
        }

        // 3. THE CLEAN VERSION: Resolve directly from the existing request-scoped provider
        var context = _serviceProvider.GetRequiredService<IApplicationDbContext>();

        var school = await context.Schools
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(s => s.UserId == userId, default);

        _cachedSchoolId = school?.Id;
        _isResolved = true;
        return _cachedSchoolId;
    }
}