using MediatR;
using SchoolScheduler.Application.Common.Interfaces;
using SchoolScheduler.Application.Assignments;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SchoolScheduler.Application.Assignments.Queries.GetClassSubjectAssignmentById;

public record GetClassSubjectAssignmentByIdQuery(Guid Id) : IRequest<ClassSubjectAssignmentDto>;

public class GetClassSubjectAssignmentByIdQueryHandler : IRequestHandler<GetClassSubjectAssignmentByIdQuery, ClassSubjectAssignmentDto>
{
    private readonly IApplicationDbContext _context;

    public GetClassSubjectAssignmentByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ClassSubjectAssignmentDto> Handle(GetClassSubjectAssignmentByIdQuery request, CancellationToken cancellationToken)
    {
        var assignment = await _context.ClassSubjectAssignments
            .Where(a => a.Id == request.Id)
            .Select(a => new ClassSubjectAssignmentDto(
                a.Id,
                a.ClassRoomId,
                a.SubjectId,
                a.TeacherId,
                a.WeeklyQuota
            ))
            .FirstOrDefaultAsync(cancellationToken);

        return assignment;
    }
}
