using MediatR;
using SchoolScheduler.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SchoolScheduler.Application.ClassRooms;

namespace SchoolScheduler.Application.ClassRooms.Queries.GetAllClassRooms;

public record GetAllClassRoomsQuery(Guid GradeId) : IRequest<List<ClassRoomDto>>;

public class GetAllClassRoomsQueryHandler : IRequestHandler<GetAllClassRoomsQuery, List<ClassRoomDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAllClassRoomsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ClassRoomDto>> Handle(GetAllClassRoomsQuery request, CancellationToken cancellationToken)
    {
        return await _context.ClassRooms
            .Where(c => c.GradeId == request.GradeId)
            .OrderBy(c => c.Name)
            .Select(c => new ClassRoomDto(c.Id, c.SchoolId, c.GradeId, c.Name))
            .ToListAsync(cancellationToken);
    }
}
