using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolScheduler.Application.Common.Interfaces;
using SchoolScheduler.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolScheduler.Application.LinkRequests.Queries.GetPendingLinkRequestsForUser;

public record PendingLinkRequestDto(
    Guid RequestId,
    string SchoolName,
    string TeacherDisplayName,
    DateTime CreatedAt
);

public record GetPendingLinkRequestsForUserQuery(string UserId) : IRequest<List<PendingLinkRequestDto>>;

public class GetPendingLinkRequestsForUserQueryHandler : IRequestHandler<GetPendingLinkRequestsForUserQuery, List<PendingLinkRequestDto>>
{
    private readonly IApplicationDbContext _context;

    public GetPendingLinkRequestsForUserQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<PendingLinkRequestDto>> Handle(GetPendingLinkRequestsForUserQuery request, CancellationToken cancellationToken)
    {
        var pendingRequests = await _context.TeacherLinkRequests
            .Where(lr => lr.UserId == request.UserId && lr.Status == LinkRequestStatus.Pending)
            .ToListAsync(cancellationToken);

        var result = new List<PendingLinkRequestDto>();

        foreach (var lr in pendingRequests)
        {
            var teacher = await _context.Teachers
                .FirstOrDefaultAsync(t => t.Id == lr.TeacherId, cancellationToken);

            if (teacher == null) continue;

            // We need to get the school name. Teacher has SchoolId.
            var school = await _context.Schools
                .FirstOrDefaultAsync(s => s.Id == teacher.SchoolId, cancellationToken);

            result.Add(new PendingLinkRequestDto(
                lr.Id,
                school?.Name ?? "Unknown School",
                teacher.DisplayName,
                lr.CreatedAt
            ));
        }

        return result;
    }
}
