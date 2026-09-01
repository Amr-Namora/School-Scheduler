using MediatR;
using SchoolScheduler.Application.Common.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SchoolScheduler.Application.Subjects.Commands.UpdateSubject;

public record UpdateSubjectCommand(Guid SubjectId, string Name) : IRequest<bool>;

public class UpdateSubjectCommandHandler : IRequestHandler<UpdateSubjectCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public UpdateSubjectCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(UpdateSubjectCommand request, CancellationToken cancellationToken)
    {
        var subject = await _context.Subjects
            .FirstOrDefaultAsync(s => s.Id == request.SubjectId, cancellationToken);

        if (subject == null)
        {
            return false; // Not found
        }

        subject.Update(request.Name);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
