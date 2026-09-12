using MediatR;
using SchoolScheduler.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolScheduler.Application.Subjects.Queries.GetSubjectsWithTeachers;

public record GetSubjectsWithTeachersQuery(Guid SchoolId) : IRequest<List<SubjectTeachersDto>>;

public class GetSubjectsWithTeachersQueryHandler : IRequestHandler<GetSubjectsWithTeachersQuery, List<SubjectTeachersDto>>
{
    private readonly IApplicationDbContext _context;

    public GetSubjectsWithTeachersQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<SubjectTeachersDto>> Handle(GetSubjectsWithTeachersQuery request, CancellationToken cancellationToken)
    {
        return await _context.Subjects
            .Where(s => s.SchoolId == request.SchoolId)
            .OrderBy(s => s.Name)
            .Select(s => new SubjectTeachersDto(
                s.Id,
                s.Name,
                _context.TeacherSubjects
                    .Where(ts => ts.SubjectId == s.Id)
                    .Select(ts => new TeacherShortDto(ts.Teacher.Id, ts.Teacher.DisplayName))
                    .ToList()
            ))
            .ToListAsync(cancellationToken);
    }
}
