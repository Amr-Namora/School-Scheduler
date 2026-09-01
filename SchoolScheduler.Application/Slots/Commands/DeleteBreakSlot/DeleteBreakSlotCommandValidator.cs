using FluentValidation;
using SchoolScheduler.Application.Slots.Commands.DeleteBreakSlot;
using SchoolScheduler.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolScheduler.Application.Slots.Commands.DeleteBreakSlot;

public class DeleteBreakSlotCommandValidator : AbstractValidator<DeleteBreakSlotCommand>
{
    public DeleteBreakSlotCommandValidator(IApplicationDbContext context)
    {
        RuleFor(x => x.SlotId).NotEmpty();

        RuleFor(x => x).MustAsync(async (cmd, cancellation) =>
        {
            var exists = await context.BreakSlots.AnyAsync(s => s.Id == cmd.SlotId, cancellation);
            return exists;
        }).WithMessage("Break slot not found.");
    }
}
