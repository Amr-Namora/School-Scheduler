using MediatR;
using SchoolScheduler.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SchoolScheduler.Application.Slots;

namespace SchoolScheduler.Application.Slots.Queries.GetAllBreakSlots;

public record GetAllBreakSlotsQuery(Guid SchoolId) : IRequest<List<BreakSlotDto>>;

public class GetAllBreakSlotsQueryHandler : IRequestHandler<GetAllBreakSlotsQuery, List<BreakSlotDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAllBreakSlotsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<BreakSlotDto>> Handle(GetAllBreakSlotsQuery request, CancellationToken cancellationToken)
    {
        return await _context.BreakSlots
            .Where(s => s.SchoolId == request.SchoolId)
            .OrderBy(s => s.DayOfWeek)
            .ThenBy(s => s.AfterSlotNumber)
            .Select(s => new BreakSlotDto(s.Id, s.SchoolId, s.DayOfWeek, s.AfterSlotNumber, s.StartTime, s.EndTime))
            .ToListAsync(cancellationToken);
    }
}
