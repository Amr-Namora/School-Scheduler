using MediatR;
using SchoolScheduler.Application.Common.Interfaces;
using SchoolScheduler.Application.Subjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SchoolScheduler.Application.Teachers.Queries.GetMySubjects;

public record MySubjectEntry(Guid SchoolId, string SchoolName, List<SubjectDto> Subjects);
public record GetMySubjectsQuery(string UserId) : IRequest<List<MySubjectEntry>>;

public class GetMySubjectsQueryHandler : IRequestHandler<GetMySubjectsQuery, List<MySubjectEntry>>
{
    private readonly IApplicationDbContext _context;

    public GetMySubjectsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<MySubjectEntry>> Handle(GetMySubjectsQuery request, CancellationToken cancellationToken)
    {
        var teachers = await _context.Teachers
            .IgnoreQueryFilters()
            .Where(t => t.UserId == request.UserId)
            .ToListAsync(cancellationToken);

        var results = new List<MySubjectEntry>();

        foreach (var teacher in teachers)
        {
            var school = await _context.Schools
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(s => s.Id == teacher.SchoolId, cancellationToken);

            var subjectIds = await _context.TeacherSubjects
                .Where(ts => ts.TeacherId == teacher.Id)
                .Select(ts => ts.SubjectId)
                .ToListAsync(cancellationToken);

            var subjects = await _context.Subjects
                .Where(s => subjectIds.Contains(s.Id))
                .OrderBy(s => s.Name)
                .Select(s => new SubjectDto(s.Id, s.SchoolId, s.Name))
                .ToListAsync(cancellationToken);

            results.Add(new MySubjectEntry(
                teacher.SchoolId,
                school?.Name ?? "Unknown School",
                subjects
            ));
        }

        return results;
    }
}
