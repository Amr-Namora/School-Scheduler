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

public record GetAllClassRoomsQuery(Guid SchoolId, Guid GradeId) : IRequest<List<ClassRoomDto>>;

public class GetAllClassRoomsQueryHandler : IRequestHandler<GetAllClassRoomsQuery, List<ClassRoomDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAllClassRoomsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ClassRoomDto>> Handle(GetAllClassRoomsQuery request, CancellationToken cancellationToken)
    {
        var gradeExists = await _context.Grades
            .AnyAsync(g => g.Id == request.GradeId && g.SchoolId == request.SchoolId, cancellationToken);

        if (!gradeExists)
        {
            throw new InvalidOperationException("Grade not found.");
        }

        return await _context.ClassRooms
            .Where(c => c.SchoolId == request.SchoolId && c.GradeId == request.GradeId)
            .OrderBy(c => c.Name)
            .Select(c => new ClassRoomDto(c.Id, c.SchoolId, c.GradeId, c.Name))
            .ToListAsync(cancellationToken);
    }
}
