using FluentValidation;
using SchoolScheduler.Application.Teachers.Commands.UpdateTeacher;
using SchoolScheduler.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolScheduler.Application.Teachers.Commands.UpdateTeacher;

public class UpdateTeacherCommandValidator : AbstractValidator<UpdateTeacherCommand>
{
    public UpdateTeacherCommandValidator(IApplicationDbContext context)
    {
        RuleFor(x => x.TeacherId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().WithMessage("Teacher name is required.");

        RuleFor(x => x).MustAsync(async (cmd, cancellation) =>
        {
            var exists = await context.Teachers.AnyAsync(t => t.Id == cmd.TeacherId, cancellation);
            return exists;
        }).WithMessage("Teacher not found.");
    }
}
