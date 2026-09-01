using FluentValidation;
using SchoolScheduler.Application.Slots.Commands.DeleteLectureSlot;
using SchoolScheduler.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolScheduler.Application.Slots.Commands.DeleteLectureSlot;

public class DeleteLectureSlotCommandValidator : AbstractValidator<DeleteLectureSlotCommand>
{
    public DeleteLectureSlotCommandValidator(IApplicationDbContext context)
    {
        RuleFor(x => x.SlotId).NotEmpty();

        RuleFor(x => x).MustAsync(async (cmd, cancellation) =>
        {
            var exists = await context.LectureSlots.AnyAsync(s => s.Id == cmd.SlotId, cancellation);
            return exists;
        }).WithMessage("Lecture slot not found.");
    }
}
