using MediatR;
using SchoolScheduler.Application.Common.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SchoolScheduler.Application.Slots.Commands.UpdateLectureSlot;

public record UpdateLectureSlotCommand(
    Guid SlotId,
    int SlotNumber,
    TimeSpan StartTime,
    TimeSpan EndTime
) : IRequest<bool>;

public class UpdateLectureSlotCommandHandler : IRequestHandler<UpdateLectureSlotCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public UpdateLectureSlotCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(UpdateLectureSlotCommand request, CancellationToken cancellationToken)
    {
        var slot = await _context.LectureSlots
            .FirstOrDefaultAsync(s => s.Id == request.SlotId, cancellationToken);

        if (slot == null)
        {
            return false; // Not found
        }

        slot.Update(request.SlotNumber, request.StartTime, request.EndTime);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
