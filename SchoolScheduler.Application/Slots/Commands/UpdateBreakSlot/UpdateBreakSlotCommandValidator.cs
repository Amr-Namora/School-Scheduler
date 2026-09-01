using FluentValidation;
using SchoolScheduler.Application.Slots.Commands.UpdateBreakSlot;
using SchoolScheduler.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolScheduler.Application.Slots.Commands.UpdateBreakSlot;

public class UpdateBreakSlotCommandValidator : AbstractValidator<UpdateBreakSlotCommand>
{
    public UpdateBreakSlotCommandValidator(IApplicationDbContext context)
    {
        RuleFor(x => x.SlotId).NotEmpty();
        RuleFor(x => x.AfterSlotNumber).GreaterThan(0);

        RuleFor(x => x).MustAsync(async (cmd, cancellation) =>
        {
            var exists = await context.BreakSlots.AnyAsync(s => s.Id == cmd.SlotId, cancellation);
            return exists;
        }).WithMessage("Break slot not found.");

        RuleFor(x => x).MustAsync(async (cmd, cancellation) =>
        {
            var slot = await context.BreakSlots.FirstOrDefaultAsync(s => s.Id == cmd.SlotId, cancellation);
            if (slot == null) return true;

            var conflict = await context.BreakSlots.AnyAsync(s =>
                s.Id != cmd.SlotId &&
                s.SchoolId == slot.SchoolId &&
                s.DayOfWeek == slot.DayOfWeek &&
                s.AfterSlotNumber == cmd.AfterSlotNumber, cancellation);

            return !conflict;
        }).WithMessage("Another break slot after this lecture slot already exists for this day.");
    }
}
