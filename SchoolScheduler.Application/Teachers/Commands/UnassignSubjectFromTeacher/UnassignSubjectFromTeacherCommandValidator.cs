using FluentValidation;
using SchoolScheduler.Application.Teachers.Commands.UnassignSubjectFromTeacher;
using SchoolScheduler.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolScheduler.Application.Teachers.Commands.UnassignSubjectFromTeacher;

public class UnassignSubjectFromTeacherCommandValidator : AbstractValidator<UnassignSubjectFromTeacherCommand>
{
    public UnassignSubjectFromTeacherCommandValidator(IApplicationDbContext context)
    {
        RuleFor(x => x.TeacherId).NotEmpty();
        RuleFor(x => x.SubjectId).NotEmpty();

        RuleFor(x => x).MustAsync(async (cmd, cancellation) =>
        {
            var exists = await context.TeacherSubjects.AnyAsync(ts => ts.TeacherId == cmd.TeacherId && ts.SubjectId == cmd.SubjectId, cancellation);
            return exists;
        }).WithMessage("Teacher is not assigned to this subject.");
    }
}
