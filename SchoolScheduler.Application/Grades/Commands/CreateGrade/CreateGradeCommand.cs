using MediatR;
using SchoolScheduler.Application.Common.Interfaces;
using SchoolScheduler.Domain.Entities;
using SchoolScheduler.Domain.Enums;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolScheduler.Application.Grades.Commands.CreateGrade;

public record CreateGradeCommand(Guid SchoolId, GradeCategory Category, int Level) : IRequest<Guid>;

public class CreateGradeCommandHandler : IRequestHandler<CreateGradeCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateGradeCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateGradeCommand request, CancellationToken cancellationToken)
    {
        var grade = new Grade(Guid.NewGuid(), request.SchoolId, request.Category, request.Level);
        _context.Grades.Add(grade);
        await _context.SaveChangesAsync(cancellationToken);
        return grade.Id;
    }
}
