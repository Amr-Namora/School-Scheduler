using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SchoolScheduler.Application.Common.Interfaces;
using SchoolScheduler.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace SchoolScheduler.Infrastructure.Persistence;

public class CurrentSchoolContext : ICurrentSchoolContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IApplicationDbContext _context;
    private Guid? _cachedSchoolId;

    public CurrentSchoolContext(IHttpContextAccessor httpContextAccessor, IApplicationDbContext context)
    {
        _httpContextAccessor = httpContextAccessor;
        _context = context;
    }

    public Guid? SchoolId => _cachedSchoolId;

    public async Task<Guid?> GetSchoolIdAsync()
    {
        if (_cachedSchoolId.HasValue) return _cachedSchoolId;

        var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            _cachedSchoolId = null;
            return null;
        }

        var school = await _context.Schools
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(s => s.UserId == userId, default);

        _cachedSchoolId = school?.Id;
        return _cachedSchoolId;
    }
}
