using MediatR;
using SchoolScheduler.Domain.Common.Exceptions;
using SchoolScheduler.Application.Common.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SchoolScheduler.Application.Schools.Commands.SoftDeleteSchool;

public record SoftDeleteSchoolCommand(Guid SchoolId) : IRequest<bool>;

public class SoftDeleteSchoolCommandHandler : IRequestHandler<SoftDeleteSchoolCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public SoftDeleteSchoolCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(SoftDeleteSchoolCommand request, CancellationToken cancellationToken)
    {
        var school = await _context.Schools
            .FirstOrDefaultAsync(s => s.Id == request.SchoolId, cancellationToken);

        if (school == null)
        {
            return false; // Not found
        }

        // Block soft-delete if dependents exist
        var hasGrades = await _context.Grades.AnyAsync(g => g.SchoolId == request.SchoolId, cancellationToken);
        var hasTeachers = await _context.Teachers.AnyAsync(t => t.SchoolId == request.SchoolId, cancellationToken);
        var hasSubjects = await _context.Subjects.AnyAsync(s => s.SchoolId == request.SchoolId, cancellationToken);
        var hasSlots = await _context.LectureSlots.AnyAsync(l => l.SchoolId == request.SchoolId, cancellationToken);

        if (hasGrades || hasTeachers || hasSubjects || hasSlots)
        {
            throw new ConflictException("Cannot soft-delete school because it has associated grades, teachers, subjects, or lecture slots.");
        }

        school.SoftDelete();
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
