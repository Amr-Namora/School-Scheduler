using MediatR;
using SchoolScheduler.Application.Common.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SchoolScheduler.Application.Slots.Commands.UpdateBreakSlot;

public record UpdateBreakSlotCommand(
    Guid SlotId,
    int AfterSlotNumber,
    TimeSpan StartTime,
    TimeSpan EndTime
) : IRequest<bool>;

public class UpdateBreakSlotCommandHandler : IRequestHandler<UpdateBreakSlotCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public UpdateBreakSlotCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(UpdateBreakSlotCommand request, CancellationToken cancellationToken)
    {
        var slot = await _context.BreakSlots
            .FirstOrDefaultAsync(s => s.Id == request.SlotId, cancellationToken);

        if (slot == null)
        {
            return false; // Not found
        }

        slot.Update(request.AfterSlotNumber, request.StartTime, request.EndTime);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
