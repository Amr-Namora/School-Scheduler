using MediatR;
using SchoolScheduler.Application.Common.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SchoolScheduler.Application.Assignments.Commands.DeleteClassSubjectAssignment;

public record DeleteClassSubjectAssignmentCommand(Guid Id) : IRequest<bool>;

public class DeleteClassSubjectAssignmentCommandHandler : IRequestHandler<DeleteClassSubjectAssignmentCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public DeleteClassSubjectAssignmentCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteClassSubjectAssignmentCommand request, CancellationToken cancellationToken)
    {
        var assignment = await _context.ClassSubjectAssignments
            .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);

        if (assignment == null)
        {
            return false;
        }

        _context.ClassSubjectAssignments.Remove(assignment);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
