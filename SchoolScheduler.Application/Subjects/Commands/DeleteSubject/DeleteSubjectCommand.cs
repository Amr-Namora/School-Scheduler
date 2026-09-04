using MediatR;
using SchoolScheduler.Domain.Common.Exceptions;
using SchoolScheduler.Application.Common.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SchoolScheduler.Application.Subjects.Commands.DeleteSubject;

public record DeleteSubjectCommand(Guid SubjectId) : IRequest<bool>;

public class DeleteSubjectCommandHandler : IRequestHandler<DeleteSubjectCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public DeleteSubjectCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteSubjectCommand request, CancellationToken cancellationToken)
    {
        var subject = await _context.Subjects
            .FirstOrDefaultAsync(s => s.Id == request.SubjectId, cancellationToken);

        if (subject == null)
        {
            return false; // Not found
        }

        // Block deletion if assignments or teacher-subject mappings exist
        var hasAssignments = await _context.ClassSubjectAssignments.AnyAsync(a => a.SubjectId == request.SubjectId, cancellationToken);
        var hasTeacherMappings = await _context.TeacherSubjects.AnyAsync(ts => ts.SubjectId == request.SubjectId, cancellationToken);

        if (hasAssignments || hasTeacherMappings)
        {
            throw new ConflictException("Cannot delete subject because it is assigned to classrooms or teachers.");
        }

        _context.Subjects.Remove(subject);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
