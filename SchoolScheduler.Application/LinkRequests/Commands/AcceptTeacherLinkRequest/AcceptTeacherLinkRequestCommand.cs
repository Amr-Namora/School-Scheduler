using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolScheduler.Application.Common.Interfaces;
using SchoolScheduler.Domain.Entities;
using SchoolScheduler.Domain.Enums;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolScheduler.Application.LinkRequests.Commands.AcceptTeacherLinkRequest;

public record AcceptTeacherLinkRequestCommand(Guid RequestId, string RespondingUserId) : IRequest<bool>;

public class AcceptTeacherLinkRequestCommandHandler : IRequestHandler<AcceptTeacherLinkRequestCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public AcceptTeacherLinkRequestCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(AcceptTeacherLinkRequestCommand request, CancellationToken cancellationToken)
    {
        var linkRequest = await _context.TeacherLinkRequests
            .FirstOrDefaultAsync(lr => lr.Id == request.RequestId, cancellationToken);

        if (linkRequest == null)
        {
            throw new InvalidOperationException("Link request not found.");
        }

        if (linkRequest.UserId != request.RespondingUserId)
        {
            throw new UnauthorizedAccessException("This request does not belong to the current user.");
        }

        if (linkRequest.Status != LinkRequestStatus.Pending)
        {
            throw new InvalidOperationException("This request has already been responded to.");
        }

        var teacher = await _context.Teachers
            .FirstOrDefaultAsync(t => t.Id == linkRequest.TeacherId, cancellationToken);

        if (teacher == null)
        {
            throw new InvalidOperationException("Associated teacher record not found.");
        }

        // This will throw if already linked
        teacher.LinkToUser(request.RespondingUserId);

        linkRequest.Respond(LinkRequestStatus.Accepted);

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
