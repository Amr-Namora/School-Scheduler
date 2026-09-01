using MediatR;
using SchoolScheduler.Application.Common.Interfaces;
using SchoolScheduler.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolScheduler.Application.Subjects.Commands.CreateSubject;

public record CreateSubjectCommand(Guid SchoolId, string Name) : IRequest<Guid>;

public class CreateSubjectCommandHandler : IRequestHandler<CreateSubjectCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateSubjectCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateSubjectCommand request, CancellationToken cancellationToken)
    {
        var subject = new Subject(Guid.NewGuid(), request.SchoolId, request.Name);
        _context.Subjects.Add(subject);
        await _context.SaveChangesAsync(cancellationToken);
        return subject.Id;
    }
}
