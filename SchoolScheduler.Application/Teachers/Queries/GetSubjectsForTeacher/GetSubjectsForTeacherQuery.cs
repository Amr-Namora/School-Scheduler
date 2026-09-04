using MediatR;
using SchoolScheduler.Domain.Common.Exceptions;
using SchoolScheduler.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SchoolScheduler.Application.Subjects;

namespace SchoolScheduler.Application.Teachers.Queries.GetSubjectsForTeacher;

public record GetSubjectsForTeacherQuery(Guid TeacherId) : IRequest<List<SubjectDto>>;

public class GetSubjectsForTeacherQueryHandler : IRequestHandler<GetSubjectsForTeacherQuery, List<SubjectDto>>
{
    private readonly IApplicationDbContext _context;

    public GetSubjectsForTeacherQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<SubjectDto>> Handle(GetSubjectsForTeacherQuery request, CancellationToken cancellationToken)
    {
        var teacherExists = await _context.Teachers
            .AnyAsync(t => t.Id == request.TeacherId, cancellationToken);

        if (!teacherExists)
        {
            throw new NotFoundException("Teacher not found.");
        }

        var subjectIds = await _context.TeacherSubjects
            .Where(ts => ts.TeacherId == request.TeacherId)
            .Select(ts => ts.SubjectId)
            .ToListAsync(cancellationToken);

        return await _context.Subjects
            .Where(s => subjectIds.Contains(s.Id))
            .OrderBy(s => s.Name)
            .Select(s => new SubjectDto(s.Id, s.SchoolId, s.Name))
            .ToListAsync(cancellationToken);
    }
}
