using MediatR;
using SchoolScheduler.Application.Common.Interfaces;
using SchoolScheduler.Application.Teachers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SchoolScheduler.Application.Teachers.Queries.GetAvailabilityForTeacher;

public record GetAvailabilityForTeacherQuery(Guid TeacherId) : IRequest<List<TeacherAvailabilityByDayDto>>;

public class GetAvailabilityForTeacherQueryHandler : IRequestHandler<GetAvailabilityForTeacherQuery, List<TeacherAvailabilityByDayDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAvailabilityForTeacherQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<TeacherAvailabilityByDayDto>> Handle(GetAvailabilityForTeacherQuery request, CancellationToken cancellationToken)
    {
        var availabilities = await _context.TeacherAvailabilities
            .Where(a => a.TeacherId == request.TeacherId)
            .ToListAsync(cancellationToken);

        return availabilities
            .GroupBy(a => a.DayOfWeek)
            .Select(g => new TeacherAvailabilityByDayDto(
                g.Key,
                g.Select(a => a.SlotNumber).OrderBy(s => s).ToList()
            ))
            .OrderBy(d => (int)d.DayOfWeek)
            .ToList();
    }
}
