using MediatR;
using SchoolScheduler.Application.Common.Interfaces;
using SchoolScheduler.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolScheduler.Application.Teachers.Commands.CreateTeacher;

public record CreateTeacherCommand(Guid SchoolId, string DisplayName, string Email) : IRequest<Guid>;

public class CreateTeacherCommandHandler : IRequestHandler<CreateTeacherCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly IUserLookupService _userLookupService;

    public CreateTeacherCommandHandler(IApplicationDbContext context, IUserLookupService userLookupService)
    {
        _context = context;
        _userLookupService = userLookupService;
    }

    public async Task<Guid> Handle(CreateTeacherCommand request, CancellationToken cancellationToken)
    {
        var teacher = new Teacher(Guid.NewGuid(), request.SchoolId, request.DisplayName, request.Email);
        _context.Teachers.Add(teacher);
        await _context.SaveChangesAsync(cancellationToken);

        var userId = await _userLookupService.FindUserIdByEmailAsync(request.Email);
        if (!string.IsNullOrEmpty(userId))
        {
            var linkRequest = new TeacherLinkRequest(teacher.Id, userId);
            _context.TeacherLinkRequests.Add(linkRequest);
            await _context.SaveChangesAsync(cancellationToken);
        }

        return teacher.Id;
    }
}
