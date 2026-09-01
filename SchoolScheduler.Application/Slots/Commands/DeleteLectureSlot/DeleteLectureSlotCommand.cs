using MediatR;
using SchoolScheduler.Application.Common.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SchoolScheduler.Application.Slots.Commands.DeleteLectureSlot;

public record DeleteLectureSlotCommand(Guid SlotId) : IRequest<bool>;

public class DeleteLectureSlotCommandHandler : IRequestHandler<DeleteLectureSlotCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public DeleteLectureSlotCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteLectureSlotCommand request, CancellationToken cancellationToken)
    {
        var slot = await _context.LectureSlots
            .FirstOrDefaultAsync(s => s.Id == request.SlotId, cancellationToken);

        if (slot == null)
        {
            return false; // Not found
        }

        _context.LectureSlots.Remove(slot);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
