using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolScheduler.Application.Common.Interfaces;
using SchoolScheduler.Domain.Entities;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SchoolScheduler.Application.LinkRequests.Commands.CreateLinkRequestsForNewUser;

public record CreateLinkRequestsForNewUserCommand(string UserId, string Email) : IRequest<int>;

public class CreateLinkRequestsForNewUserCommandHandler : IRequestHandler<CreateLinkRequestsForNewUserCommand, int>
{
    private readonly IApplicationDbContext _context;

    public CreateLinkRequestsForNewUserCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateLinkRequestsForNewUserCommand request, CancellationToken cancellationToken)
    {
        // Find all teachers with matching email and no UserId, bypassing tenant filters
        var matchingTeachers = await _context.Teachers
            .IgnoreQueryFilters()
            .Where(t => t.Email.ToLower() == request.Email.ToLower() && t.UserId == null)
            .ToListAsync(cancellationToken);

        if (!matchingTeachers.Any())
        {
            return 0;
        }

        foreach (var teacher in matchingTeachers)
        {
            var linkRequest = new TeacherLinkRequest(teacher.Id, request.UserId);
            _context.TeacherLinkRequests.Add(linkRequest);
        }

        await _context.SaveChangesAsync(cancellationToken);
        return matchingTeachers.Count;
    }
}
