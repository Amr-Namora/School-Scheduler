using FluentValidation;
using SchoolScheduler.Application.Grades.Commands.DeleteGrade;
using SchoolScheduler.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolScheduler.Application.Grades.Commands.DeleteGrade;

public class DeleteGradeCommandValidator : AbstractValidator<DeleteGradeCommand>
{
    public DeleteGradeCommandValidator(IApplicationDbContext context)
    {
        RuleFor(x => x.GradeId).NotEmpty();

        RuleFor(x => x).MustAsync(async (cmd, cancellation) =>
        {
            var exists = await context.Grades.AnyAsync(g => g.Id == cmd.GradeId, cancellation);
            return exists;
        }).WithMessage("Grade not found.");
    }
}
