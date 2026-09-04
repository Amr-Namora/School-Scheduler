using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SchoolScheduler.Application.Common.Interfaces;
using SchoolScheduler.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace SchoolScheduler.Infrastructure.Persistence;

public class CurrentSchoolContext : ICurrentSchoolContext
{
    private readonly ICurrentUserContext _currentUserContext;
    private readonly IApplicationDbContext _context;
    private Guid? _cachedSchoolId;
    private bool _isResolved = false;

    public CurrentSchoolContext(ICurrentUserContext currentUserContext, IApplicationDbContext context)
    {
        _currentUserContext = currentUserContext;
        _context = context;
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

        // Role Validation: Only "School" role can resolve a school context
        if (_currentUserContext.Role != "School")
        {
            throw new UnauthorizedAccessException("School context is only available to users with the 'School' role.");
        }

        var userId = _currentUserContext.UserId;
        if (string.IsNullOrEmpty(userId))
        {
            _cachedSchoolId = null;
            _isResolved = true;
            return null;
        }

        var school = await _context.Schools
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(s => s.UserId == userId, default);

        _cachedSchoolId = school?.Id;
        _isResolved = true;
        return _cachedSchoolId;
    }
}
