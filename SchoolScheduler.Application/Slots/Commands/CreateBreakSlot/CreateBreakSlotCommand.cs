using MediatR;
using SchoolScheduler.Application.Common.Interfaces;
using SchoolScheduler.Domain.Entities;
using SchoolScheduler.Domain.Enums;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolScheduler.Application.Slots.Commands.CreateBreakSlot;

public record CreateBreakSlotCommand(
    Guid SchoolId,
    SchoolDayOfWeek DayOfWeek,
    int AfterSlotNumber,
    TimeSpan StartTime,
    TimeSpan EndTime
) : IRequest<Guid>;

public class CreateBreakSlotCommandHandler : IRequestHandler<CreateBreakSlotCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateBreakSlotCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateBreakSlotCommand request, CancellationToken cancellationToken)
    {
        var slot = new BreakSlot(
            Guid.NewGuid(),
            request.SchoolId,
            request.DayOfWeek,
            request.AfterSlotNumber,
            request.StartTime,
            request.EndTime);

        _context.BreakSlots.Add(slot);
        await _context.SaveChangesAsync(cancellationToken);

        return slot.Id;
    }
}
