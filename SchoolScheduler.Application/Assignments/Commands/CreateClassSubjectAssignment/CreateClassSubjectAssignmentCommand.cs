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
        var classRoom = await _context.ClassRooms
            .FirstOrDefaultAsync(c => c.Id == request.ClassRoomId, cancellationToken);

        if (classRoom == null)
        {
            throw new InvalidOperationException("Class room not found.");
        }

        var assignment = new ClassSubjectAssignment(
            Guid.NewGuid(),
            classRoom.SchoolId,
            request.ClassRoomId,
            request.SubjectId,
            request.TeacherId,
            request.WeeklyQuota);

        _context.ClassSubjectAssignments.Add(assignment);
        await _context.SaveChangesAsync(cancellationToken);
        return assignment.Id;
    }
}
