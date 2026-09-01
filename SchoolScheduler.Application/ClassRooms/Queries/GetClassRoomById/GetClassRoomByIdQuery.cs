using MediatR;
using SchoolScheduler.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
using SchoolScheduler.Application.ClassRooms;

namespace SchoolScheduler.Application.ClassRooms.Queries.GetClassRoomById;

public record GetClassRoomByIdQuery(Guid Id) : IRequest<ClassRoomDto>;

public class GetClassRoomByIdQueryHandler : IRequestHandler<GetClassRoomByIdQuery, ClassRoomDto>
{
    private readonly IApplicationDbContext _context;

    public GetClassRoomByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ClassRoomDto> Handle(GetClassRoomByIdQuery request, CancellationToken cancellationToken)
    {
        var classroom = await _context.ClassRooms
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (classroom == null)
        {
            return null;
        }

        return new ClassRoomDto(classroom.Id, classroom.SchoolId, classroom.GradeId, classroom.Name);
    }
}
