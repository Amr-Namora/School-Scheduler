using MediatR;
using SchoolScheduler.Domain.Common.Exceptions;
using SchoolScheduler.Application.Common.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SchoolScheduler.Application.Teachers.Commands.UnassignSubjectFromTeacher;

public record UnassignSubjectFromTeacherCommand(Guid TeacherId, Guid SubjectId) : IRequest<bool>;

public class UnassignSubjectFromTeacherCommandHandler : IRequestHandler<UnassignSubjectFromTeacherCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public UnassignSubjectFromTeacherCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(UnassignSubjectFromTeacherCommand request, CancellationToken cancellationToken)
    {
        var mapping = await _context.TeacherSubjects
            .FirstOrDefaultAsync(ts => ts.TeacherId == request.TeacherId && ts.SubjectId == request.SubjectId, cancellationToken);

        if (mapping == null)
        {
            return false; // Not found
        }

        // Block removal if the teacher is currently assigned to a class for this subject
        var hasAssignments = await _context.ClassSubjectAssignments.AnyAsync(a => a.TeacherId == request.TeacherId && a.SubjectId == request.SubjectId, cancellationToken);

        if (hasAssignments)
        {
            throw new ConflictException("Cannot unassign subject from teacher because they are already assigned to a classroom for this subject.");
        }

        _context.TeacherSubjects.Remove(mapping);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
