using MediatR;
using SchoolScheduler.Application.Common.Interfaces;
using SchoolScheduler.Domain.Enums;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SchoolScheduler.Application.Grades.Commands.UpdateGrade;

public record UpdateGradeCommand(Guid GradeId, GradeCategory Category, int Level) : IRequest<bool>;

public class UpdateGradeCommandHandler : IRequestHandler<UpdateGradeCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public UpdateGradeCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(UpdateGradeCommand request, CancellationToken cancellationToken)
    {
        var grade = await _context.Grades
            .FirstOrDefaultAsync(g => g.Id == request.GradeId, cancellationToken);

        if (grade == null)
        {
            return false; // Not found
        }

        grade.Update(request.Category, request.Level);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
