using MediatR;
using SchoolScheduler.Application.Common.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SchoolScheduler.Application.Teachers.Commands.UpdateTeacher;

public record UpdateTeacherCommand(Guid TeacherId, string Name) : IRequest<bool>;

public class UpdateTeacherCommandHandler : IRequestHandler<UpdateTeacherCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public UpdateTeacherCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(UpdateTeacherCommand request, CancellationToken cancellationToken)
    {
        var teacher = await _context.Teachers
            .FirstOrDefaultAsync(t => t.Id == request.TeacherId, cancellationToken);

        if (teacher == null)
        {
            return false; // Not found
        }

        teacher.Update(request.Name);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
