using MediatR;
using SchoolScheduler.Application.Common.Interfaces;
using SchoolScheduler.Application.Teachers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SchoolScheduler.Application.Teachers.Queries.GetMyAvailability;

public record MyAvailabilityEntry(Guid SchoolId, string SchoolName, List<TeacherAvailabilityByDayDto> Availability);
public record GetMyAvailabilityQuery(string UserId) : IRequest<List<MyAvailabilityEntry>>;

public class GetMyAvailabilityQueryHandler : IRequestHandler<GetMyAvailabilityQuery, List<MyAvailabilityEntry>>
{
    private readonly IApplicationDbContext _context;

    public GetMyAvailabilityQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<MyAvailabilityEntry>> Handle(GetMyAvailabilityQuery request, CancellationToken cancellationToken)
    {
        var teachers = await _context.Teachers
            .IgnoreQueryFilters()
            .Where(t => t.UserId == request.UserId)
            .ToListAsync(cancellationToken);

        var results = new List<MyAvailabilityEntry>();

        foreach (var teacher in teachers)
        {
            var school = await _context.Schools
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(s => s.Id == teacher.SchoolId, cancellationToken);

            var availabilities = await _context.TeacherAvailabilities
                .Where(a => a.TeacherId == teacher.Id)
                .ToListAsync(cancellationToken);

            var availabilityByDay = availabilities
                .GroupBy(a => a.DayOfWeek)
                .Select(g => new TeacherAvailabilityByDayDto(
                    g.Key,
                    g.Select(a => a.SlotNumber).OrderBy(s => s).ToList()
                ))
                .OrderBy(d => (int)d.DayOfWeek)
                .ToList();

            results.Add(new MyAvailabilityEntry(
                teacher.SchoolId,
                school?.Name ?? "Unknown School",
                availabilityByDay
            ));
        }

        return results;
    }
}
