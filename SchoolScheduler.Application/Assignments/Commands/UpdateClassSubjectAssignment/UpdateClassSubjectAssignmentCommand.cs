using MediatR;
using SchoolScheduler.Application.Common.Interfaces;
using SchoolScheduler.Domain.Entities;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SchoolScheduler.Application.Assignments.Commands.UpdateClassSubjectAssignment;

public record UpdateClassSubjectAssignmentCommand(
    Guid Id,
    Guid TeacherId,
    Guid SubjectId,
    int WeeklyQuota
) : IRequest<bool>;

public class UpdateClassSubjectAssignmentCommandHandler : IRequestHandler<UpdateClassSubjectAssignmentCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public UpdateClassSubjectAssignmentCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(UpdateClassSubjectAssignmentCommand request, CancellationToken cancellationToken)
    {
        var assignment = await _context.ClassSubjectAssignments
            .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);

        if (assignment == null)
        {
            return false;
        }

        assignment.Update(request.TeacherId, request.SubjectId, request.WeeklyQuota);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
