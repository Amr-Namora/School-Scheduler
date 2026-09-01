using FluentValidation;
using SchoolScheduler.Application.Teachers.Commands.DeleteTeacherAvailabilitySlot;
using SchoolScheduler.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolScheduler.Application.Teachers.Commands.DeleteTeacherAvailabilitySlot;

public class DeleteTeacherAvailabilitySlotCommandValidator : AbstractValidator<DeleteTeacherAvailabilitySlotCommand>
{
    public DeleteTeacherAvailabilitySlotCommandValidator(IApplicationDbContext context)
    {
        RuleFor(x => x.TeacherId).NotEmpty();
        RuleFor(x => x.SlotNumber).GreaterThan(0);

        RuleFor(x => x).MustAsync(async (cmd, cancellation) =>
        {
            var exists = await context.TeacherAvailabilities
                .AnyAsync(a => a.TeacherId == cmd.TeacherId
                            && a.DayOfWeek == cmd.DayOfWeek
                            && a.SlotNumber == cmd.SlotNumber, cancellation);
            return exists;
        }).WithMessage("The specified availability slot was not found for this teacher.");
    }
}
