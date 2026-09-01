using MediatR;
using SchoolScheduler.Application.Common.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SchoolScheduler.Application.Schools.Commands.DeleteSchool;

public record DeleteSchoolCommand(Guid SchoolId) : IRequest<bool>;

public class DeleteSchoolCommandHandler : IRequestHandler<DeleteSchoolCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public DeleteSchoolCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteSchoolCommand request, CancellationToken cancellationToken)
    {
        var school = await _context.Schools
            .FirstOrDefaultAsync(s => s.Id == request.SchoolId, cancellationToken);

        if (school == null)
        {
            return false; // Not found
        }

        // Block deletion if dependents exist
        var hasGrades = await _context.Grades.AnyAsync(g => g.SchoolId == request.SchoolId, cancellationToken);
        var hasTeachers = await _context.Teachers.AnyAsync(t => t.SchoolId == request.SchoolId, cancellationToken);
        var hasSubjects = await _context.Subjects.AnyAsync(s => s.SchoolId == request.SchoolId, cancellationToken);
        var hasSlots = await _context.LectureSlots.AnyAsync(l => l.SchoolId == request.SchoolId, cancellationToken);

        if (hasGrades || hasTeachers || hasSubjects || hasSlots)
        {
            throw new InvalidOperationException("Cannot delete school because it has associated grades, teachers, subjects, or lecture slots.");
        }

        _context.Schools.Remove(school);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
