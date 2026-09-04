using MediatR;
using SchoolScheduler.Domain.Common.Exceptions;
using SchoolScheduler.Application.Common.Interfaces;
using SchoolScheduler.Application.Teachers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SchoolScheduler.Application.Teachers.Queries.GetAvailabilityForTeacher;

public record GetAvailabilityForTeacherQuery(Guid SchoolId, Guid TeacherId) : IRequest<List<TeacherAvailabilityByDayDto>>;

public class GetAvailabilityForTeacherQueryHandler : IRequestHandler<GetAvailabilityForTeacherQuery, List<TeacherAvailabilityByDayDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAvailabilityForTeacherQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<TeacherAvailabilityByDayDto>> Handle(GetAvailabilityForTeacherQuery request, CancellationToken cancellationToken)
    {
        // Ensure the teacher belongs to the school before returning availability
        var teacherExists = await _context.Teachers
            .AnyAsync(t => t.Id == request.TeacherId && t.SchoolId == request.SchoolId, cancellationToken);

        if (!teacherExists)
        {
            throw new NotFoundException("Teacher not found.");
        }

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
