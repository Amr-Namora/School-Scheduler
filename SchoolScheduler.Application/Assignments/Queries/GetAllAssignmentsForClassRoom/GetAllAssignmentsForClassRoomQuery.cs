using MediatR;
using SchoolScheduler.Application.Common.Interfaces;
using SchoolScheduler.Application.Assignments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SchoolScheduler.Application.Assignments.Queries.GetAllAssignmentsForClassRoom;

public record GetAllAssignmentsForClassRoomQuery(Guid SchoolId, Guid ClassRoomId) : IRequest<List<ClassSubjectAssignmentDto>>;

public class GetAllAssignmentsForClassRoomQueryHandler : IRequestHandler<GetAllAssignmentsForClassRoomQuery, List<ClassSubjectAssignmentDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAllAssignmentsForClassRoomQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ClassSubjectAssignmentDto>> Handle(GetAllAssignmentsForClassRoomQuery request, CancellationToken cancellationToken)
    {
        // Ensure the classroom belongs to the school
        var classroomExists = await _context.ClassRooms
            .AnyAsync(c => c.Id == request.ClassRoomId && c.SchoolId == request.SchoolId, cancellationToken);

        if (!classroomExists)
        {
            throw new InvalidOperationException("Class room not found.");
        }

        return await _context.ClassSubjectAssignments
            .Where(a => a.ClassRoomId == request.ClassRoomId)
            .Select(a => new ClassSubjectAssignmentDto(
                a.Id,
                a.ClassRoomId,
                a.SubjectId,
                a.TeacherId,
                a.WeeklyQuota
            ))
            .ToListAsync(cancellationToken);
    }
}
