using MediatR;
using SchoolScheduler.Domain.Common.Exceptions;
using SchoolScheduler.Application.Common.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SchoolScheduler.Application.Grades.Commands.DeleteGrade;

public record DeleteGradeCommand(Guid GradeId) : IRequest<bool>;

public class DeleteGradeCommandHandler : IRequestHandler<DeleteGradeCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public DeleteGradeCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteGradeCommand request, CancellationToken cancellationToken)
    {
        var grade = await _context.Grades
            .FirstOrDefaultAsync(g => g.Id == request.GradeId, cancellationToken);

        if (grade == null)
        {
            return false; // Not found
        }

        // Block deletion if classrooms exist
        var hasRooms = await _context.ClassRooms.AnyAsync(c => c.GradeId == request.GradeId, cancellationToken);

        if (hasRooms)
        {
            throw new ConflictException("Cannot delete grade because it has associated classrooms.");
        }

        _context.Grades.Remove(grade);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
