using MediatR;
using SchoolScheduler.Application.Common.Interfaces;
using SchoolScheduler.Domain.Entities;
using SchoolScheduler.Domain.Enums;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolScheduler.Application.Slots.Commands.CreateLectureSlot;

public record CreateLectureSlotCommand(
    Guid SchoolId,
    SchoolDayOfWeek DayOfWeek,
    int SlotNumber,
    TimeSpan StartTime,
    TimeSpan EndTime
) : IRequest<Guid>;

public class CreateLectureSlotCommandHandler : IRequestHandler<CreateLectureSlotCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateLectureSlotCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateLectureSlotCommand request, CancellationToken cancellationToken)
    {
        var slot = new LectureSlot(
            Guid.NewGuid(),
            request.SchoolId,
            request.DayOfWeek,
            request.SlotNumber,
            request.StartTime,
            request.EndTime);

        _context.LectureSlots.Add(slot);
        await _context.SaveChangesAsync(cancellationToken);

        return slot.Id;
    }
}
