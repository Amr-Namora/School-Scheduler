using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolScheduler.Application.Common.Interfaces;
using SchoolScheduler.Domain.Entities;
using SchoolScheduler.Domain.Enums;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolScheduler.Application.LinkRequests.Commands.DeclineTeacherLinkRequest;

public record DeclineTeacherLinkRequestCommand(Guid RequestId, string RespondingUserId) : IRequest<bool>;

public class DeclineTeacherLinkRequestCommandHandler : IRequestHandler<DeclineTeacherLinkRequestCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public DeclineTeacherLinkRequestCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeclineTeacherLinkRequestCommand request, CancellationToken cancellationToken)
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

        linkRequest.Respond(LinkRequestStatus.Declined);

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
