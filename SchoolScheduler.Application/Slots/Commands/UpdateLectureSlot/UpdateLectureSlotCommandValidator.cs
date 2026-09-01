using FluentValidation;
using SchoolScheduler.Application.Slots.Commands.UpdateLectureSlot;
using SchoolScheduler.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolScheduler.Application.Slots.Commands.UpdateLectureSlot;

public class UpdateLectureSlotCommandValidator : AbstractValidator<UpdateLectureSlotCommand>
{
    public UpdateLectureSlotCommandValidator(IApplicationDbContext context)
    {
        RuleFor(x => x.SlotId).NotEmpty();
        RuleFor(x => x.SlotNumber).GreaterThan(0);

        RuleFor(x => x).MustAsync(async (cmd, cancellation) =>
        {
            var exists = await context.LectureSlots.AnyAsync(s => s.Id == cmd.SlotId, cancellation);
            return exists;
        }).WithMessage("Lecture slot not found.");

        RuleFor(x => x).MustAsync(async (cmd, cancellation) =>
        {
            var slot = await context.LectureSlots.FirstOrDefaultAsync(s => s.Id == cmd.SlotId, cancellation);
            if (slot == null) return true;

            var conflict = await context.LectureSlots.AnyAsync(s =>
                s.Id != cmd.SlotId &&
                s.SchoolId == slot.SchoolId &&
                s.DayOfWeek == slot.DayOfWeek &&
                s.SlotNumber == cmd.SlotNumber, cancellation);

            return !conflict;
        }).WithMessage("Another lecture slot with this number already exists for this day.");
    }
}
