using MediatR;
using SchoolScheduler.Application.Common.Interfaces;
using SchoolScheduler.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolScheduler.Application.Teachers.Commands.CreateTeacher;

public record CreateTeacherCommand(Guid SchoolId, string Name) : IRequest<Guid>;

public class CreateTeacherCommandHandler : IRequestHandler<CreateTeacherCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateTeacherCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateTeacherCommand request, CancellationToken cancellationToken)
    {
        var teacher = new Teacher(Guid.NewGuid(), request.SchoolId, request.Name);
        _context.Teachers.Add(teacher);
        await _context.SaveChangesAsync(cancellationToken);
        return teacher.Id;
    }
}
