using FluentValidation;
using SchoolScheduler.Application.Schools.Commands.UpdateSchool;
using SchoolScheduler.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolScheduler.Application.Schools.Commands.UpdateSchool;

public class UpdateSchoolCommandValidator : AbstractValidator<UpdateSchoolCommand>
{
    public UpdateSchoolCommandValidator(IApplicationDbContext context)
    {
        RuleFor(x => x.SchoolId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().WithMessage("School name is required.");
        RuleFor(x => x.LecturesPerDay).GreaterThan(0).WithMessage("Lectures per day must be at least 1.");

        RuleFor(x => x).MustAsync(async (cmd, cancellation) =>
        {
            var exists = await context.Schools.AnyAsync(s => s.Id == cmd.SchoolId, cancellation);
            return exists;
        }).WithMessage("School not found.");
    }
}
