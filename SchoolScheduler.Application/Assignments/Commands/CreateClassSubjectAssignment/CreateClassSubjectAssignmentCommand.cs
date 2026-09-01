using MediatR;
using SchoolScheduler.Application.Common.Interfaces;
using SchoolScheduler.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolScheduler.Application.Assignments.Commands.CreateClassSubjectAssignment;

public record CreateClassSubjectAssignmentCommand(
    Guid ClassRoomId,
    Guid SubjectId,
    Guid TeacherId,
    int WeeklyQuota
) : IRequest<Guid>;

public class CreateClassSubjectAssignmentCommandHandler : IRequestHandler<CreateClassSubjectAssignmentCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateClassSubjectAssignmentCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateClassSubjectAssignmentCommand request, CancellationToken cancellationToken)
    {
        var assignment = new ClassSubjectAssignment(
            Guid.NewGuid(),
            request.ClassRoomId,
            request.SubjectId,
            request.TeacherId,
            request.WeeklyQuota);

        _context.ClassSubjectAssignments.Add(assignment);
        await _context.SaveChangesAsync(cancellationToken);
        return assignment.Id;
    }
}
