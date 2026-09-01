using MediatR;
using SchoolScheduler.Application.Common.Interfaces;
using SchoolScheduler.Domain.Entities;
using SchoolScheduler.Domain.Enums;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SchoolScheduler.Application.Teachers.Commands.ClearTeacherAvailability;

public record ClearTeacherAvailabilityCommand(
    Guid TeacherId,
    SchoolDayOfWeek? DayOfWeek = null
) : IRequest<bool>;

public class ClearTeacherAvailabilityCommandHandler : IRequestHandler<ClearTeacherAvailabilityCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public ClearTeacherAvailabilityCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(ClearTeacherAvailabilityCommand request, CancellationToken cancellationToken)
    {
        var query = _context.TeacherAvailabilities.Where(a => a.TeacherId == request.TeacherId);

        if (request.DayOfWeek.HasValue)
        {
            query = query.Where(a => a.DayOfWeek == request.DayOfWeek.Value);
        }

        var toRemove = await query.ToListAsync(cancellationToken);

        if (!toRemove.Any())
        {
            return false;
        }

        foreach (var item in toRemove)
        {
            _context.TeacherAvailabilities.Remove(item);
        }

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
