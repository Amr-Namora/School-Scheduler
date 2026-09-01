using MediatR;
using SchoolScheduler.Application.Common.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SchoolScheduler.Application.ClassRooms.Commands.UpdateClassRoom;

public record UpdateClassRoomCommand(Guid ClassRoomId, string Name) : IRequest<bool>;

public class UpdateClassRoomCommandHandler : IRequestHandler<UpdateClassRoomCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public UpdateClassRoomCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(UpdateClassRoomCommand request, CancellationToken cancellationToken)
    {
        var classroom = await _context.ClassRooms
            .FirstOrDefaultAsync(c => c.Id == request.ClassRoomId, cancellationToken);

        if (classroom == null)
        {
            return false; // Not found
        }

        classroom.Update(request.Name);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
