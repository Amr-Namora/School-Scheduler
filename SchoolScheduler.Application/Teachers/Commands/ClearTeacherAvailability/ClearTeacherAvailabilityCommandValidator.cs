using FluentValidation;
using SchoolScheduler.Application.Teachers.Commands.ClearTeacherAvailability;
using SchoolScheduler.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolScheduler.Application.Teachers.Commands.ClearTeacherAvailability;

public class ClearTeacherAvailabilityCommandValidator : AbstractValidator<ClearTeacherAvailabilityCommand>
{
    public ClearTeacherAvailabilityCommandValidator(IApplicationDbContext context)
    {
        RuleFor(x => x.TeacherId).NotEmpty();

        RuleFor(x => x).MustAsync(async (cmd, cancellation) =>
        {
            var exists = await context.Teachers.AnyAsync(t => t.Id == cmd.TeacherId, cancellation);
            return exists;
        }).WithMessage("Teacher not found.");
    }
}
