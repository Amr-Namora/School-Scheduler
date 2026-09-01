using FluentValidation;
using SchoolScheduler.Application.Slots.Commands.CreateBreakSlot;
using SchoolScheduler.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolScheduler.Application.Slots.Commands.CreateBreakSlot;

public class CreateBreakSlotCommandValidator : AbstractValidator<CreateBreakSlotCommand>
{
    public CreateBreakSlotCommandValidator(IApplicationDbContext context)
    {
        RuleFor(x => x.SchoolId).NotEmpty();
        RuleFor(x => x.AfterSlotNumber).GreaterThan(0);

        RuleFor(x => x).MustAsync(async (cmd, cancellation) =>
        {
            var exists = await context.BreakSlots.AnyAsync(s =>
                s.SchoolId == cmd.SchoolId &&
                s.DayOfWeek == cmd.DayOfWeek &&
                s.AfterSlotNumber == cmd.AfterSlotNumber, cancellation);
            return !exists;
        }).WithMessage("A break slot after this lecture slot already exists for this day.");
    }
}
