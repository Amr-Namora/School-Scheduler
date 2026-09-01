using MediatR;
using SchoolScheduler.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
using SchoolScheduler.Application.Slots;

namespace SchoolScheduler.Application.Slots.Queries.GetBreakSlotById;

public record GetBreakSlotByIdQuery(Guid Id) : IRequest<BreakSlotDto>;

public class GetBreakSlotByIdQueryHandler : IRequestHandler<GetBreakSlotByIdQuery, BreakSlotDto>
{
    private readonly IApplicationDbContext _context;

    public GetBreakSlotByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<BreakSlotDto> Handle(GetBreakSlotByIdQuery request, CancellationToken cancellationToken)
    {
        var slot = await _context.BreakSlots
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

        if (slot == null)
        {
            return null;
        }

        return new BreakSlotDto(slot.Id, slot.SchoolId, slot.DayOfWeek, slot.AfterSlotNumber, slot.StartTime, slot.EndTime);
    }
}
