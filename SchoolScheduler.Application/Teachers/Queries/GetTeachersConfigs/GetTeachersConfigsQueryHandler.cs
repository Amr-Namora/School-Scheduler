using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolScheduler.Application.Common;
using SchoolScheduler.Application.Common.Interfaces;
using SchoolScheduler.Application.Teachers.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolScheduler.Application.Teachers.Queries.GetTeachersConfigs;

public class GetTeachersConfigsQueryHandler : IRequestHandler<GetTeachersConfigsQuery, PaginatedList<TeacherConfigDto>>
{
    private readonly IApplicationDbContext _context;

    public GetTeachersConfigsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedList<TeacherConfigDto>> Handle(GetTeachersConfigsQuery request, CancellationToken cancellationToken)
    {
        var teacherQuery = _context.Teachers
            .Where(t => t.SchoolId == request.SchoolId);

        var totalCount = await teacherQuery.CountAsync(cancellationToken);

        var items = await teacherQuery
            .OrderBy(t => t.DisplayName)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(t => new TeacherConfigDto(
                t.Id,
                t.DisplayName,
                t.Email,
                _context.TeacherSubjects
                    .Where(ts => ts.TeacherId == t.Id)
                    .Select(ts => new SubjectConfigDto(ts.Subject.Id, ts.Subject.Name))
                    .ToList(),
                _context.TeacherAvailabilities
                    .Where(ta => ta.TeacherId == t.Id)
                    .Select(ta => new AvailabilityConfigDto((int)ta.DayOfWeek, ta.SlotNumber))
                    .ToList()
            ))
            .ToListAsync(cancellationToken);

        return new PaginatedList<TeacherConfigDto>(
            items,
            totalCount,
            request.Page,
            request.PageSize
        );
    }
}
