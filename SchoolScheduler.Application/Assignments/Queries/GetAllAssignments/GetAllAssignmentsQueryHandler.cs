using MediatR;
using SchoolScheduler.Application.Common.Interfaces;
using SchoolScheduler.Application.Assignments;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolScheduler.Application.Assignments.Queries.GetAllAssignments;

public class GetAllAssignmentsQueryHandler : IRequestHandler<GetAllAssignmentsQuery, List<ClassSubjectAssignmentDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAllAssignmentsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ClassSubjectAssignmentDto>> Handle(GetAllAssignmentsQuery request, CancellationToken cancellationToken)
    {
        return await _context.ClassSubjectAssignments
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
