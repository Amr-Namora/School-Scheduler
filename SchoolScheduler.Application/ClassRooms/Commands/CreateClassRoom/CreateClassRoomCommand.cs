using MediatR;
using SchoolScheduler.Application.Common.Interfaces;
using SchoolScheduler.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolScheduler.Application.ClassRooms.Commands.CreateClassRoom;

public record CreateClassRoomCommand(Guid SchoolId, Guid GradeId, string Name) : IRequest<Guid>;

public class CreateClassRoomCommandHandler : IRequestHandler<CreateClassRoomCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateClassRoomCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateClassRoomCommand request, CancellationToken cancellationToken)
    {
        var classRoom = new ClassRoom(Guid.NewGuid(), request.SchoolId, request.GradeId, request.Name);
        _context.ClassRooms.Add(classRoom);
        await _context.SaveChangesAsync(cancellationToken);
        return classRoom.Id;
    }
}
