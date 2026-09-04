using MediatR;
using SchoolScheduler.Domain.Common.Exceptions;
using SchoolScheduler.Application.Common.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SchoolScheduler.Application.Teachers.Commands.DeleteTeacher;

public record DeleteTeacherCommand(Guid TeacherId) : IRequest<bool>;

public class DeleteTeacherCommandHandler : IRequestHandler<DeleteTeacherCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public DeleteTeacherCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteTeacherCommand request, CancellationToken cancellationToken)
    {
        var teacher = await _context.Teachers
            .FirstOrDefaultAsync(t => t.Id == request.TeacherId, cancellationToken);

        if (teacher == null)
        {
            return false; // Not found
        }

        // Block deletion if assignments or availability exist
        var hasAssignments = await _context.ClassSubjectAssignments.AnyAsync(a => a.TeacherId == request.TeacherId, cancellationToken);
        var hasAvailability = await _context.TeacherAvailabilities.AnyAsync(av => av.TeacherId == request.TeacherId, cancellationToken);

        if (hasAssignments || hasAvailability)
        {
            throw new ConflictException("Cannot delete teacher because they have associated class assignments or availability records.");
        }

        _context.Teachers.Remove(teacher);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
