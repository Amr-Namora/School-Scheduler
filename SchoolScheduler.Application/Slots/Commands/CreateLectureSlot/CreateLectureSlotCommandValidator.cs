using FluentValidation;
using SchoolScheduler.Application.Slots.Commands.CreateLectureSlot;
using SchoolScheduler.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolScheduler.Application.Slots.Commands.CreateLectureSlot;

public class CreateLectureSlotCommandValidator : AbstractValidator<CreateLectureSlotCommand>
{
    public CreateLectureSlotCommandValidator(IApplicationDbContext context)
    {
        RuleFor(x => x.SchoolId).NotEmpty();
        RuleFor(x => x.SlotNumber).GreaterThan(0);

        RuleFor(x => x).MustAsync(async (cmd, cancellation) =>
        {
            var exists = await context.LectureSlots.AnyAsync(s =>
                s.SchoolId == cmd.SchoolId &&
                s.DayOfWeek == cmd.DayOfWeek &&
                s.SlotNumber == cmd.SlotNumber, cancellation);
            return !exists;
        }).WithMessage("A lecture slot with this number already exists for this day.");
    }
}
