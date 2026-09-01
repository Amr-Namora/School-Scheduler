using MediatR;
using SchoolScheduler.Application.Common.Interfaces;
using SchoolScheduler.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolScheduler.Application.Teachers.Commands.AssignSubjectsToTeacher;

public record AssignSubjectsToTeacherCommand(Guid TeacherId, List<Guid> SubjectIds) : IRequest<bool>;

public class AssignSubjectsToTeacherCommandHandler : IRequestHandler<AssignSubjectsToTeacherCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public AssignSubjectsToTeacherCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(AssignSubjectsToTeacherCommand request, CancellationToken cancellationToken)
    {
        foreach (var subjectId in request.SubjectIds)
        {
            var exists = await _context.TeacherSubjects
                .AnyAsync(ts => ts.TeacherId == request.TeacherId && ts.SubjectId == subjectId, cancellationToken);

            if (!exists)
            {
                _context.TeacherSubjects.Add(new TeacherSubject(request.TeacherId, subjectId));
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
