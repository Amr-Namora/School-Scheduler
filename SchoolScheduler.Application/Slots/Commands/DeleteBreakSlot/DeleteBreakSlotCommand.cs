using MediatR;
using SchoolScheduler.Application.Common.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SchoolScheduler.Application.Slots.Commands.DeleteBreakSlot;

public record DeleteBreakSlotCommand(Guid SlotId) : IRequest<bool>;

public class DeleteBreakSlotCommandHandler : IRequestHandler<DeleteBreakSlotCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public DeleteBreakSlotCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteBreakSlotCommand request, CancellationToken cancellationToken)
    {
        var slot = await _context.BreakSlots
            .FirstOrDefaultAsync(s => s.Id == request.SlotId, cancellationToken);

        if (slot == null)
        {
            return false; // Not found
        }

        _context.BreakSlots.Remove(slot);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
