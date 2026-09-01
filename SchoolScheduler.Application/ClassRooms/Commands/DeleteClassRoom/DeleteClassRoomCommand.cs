using MediatR;
using SchoolScheduler.Application.Common.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SchoolScheduler.Application.ClassRooms.Commands.DeleteClassRoom;

public record DeleteClassRoomCommand(Guid ClassRoomId) : IRequest<bool>;

public class DeleteClassRoomCommandHandler : IRequestHandler<DeleteClassRoomCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public DeleteClassRoomCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteClassRoomCommand request, CancellationToken cancellationToken)
    {
        var classroom = await _context.ClassRooms
            .FirstOrDefaultAsync(c => c.Id == request.ClassRoomId, cancellationToken);

        if (classroom == null)
        {
            return false; // Not found
        }

        // Block deletion if assignments exist
        var hasAssignments = await _context.ClassSubjectAssignments.AnyAsync(a => a.ClassRoomId == request.ClassRoomId, cancellationToken);

        if (hasAssignments)
        {
            throw new InvalidOperationException("Cannot delete class room because it has associated subject assignments.");
        }

        _context.ClassRooms.Remove(classroom);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
