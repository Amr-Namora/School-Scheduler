using MediatR;
using SchoolScheduler.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SchoolScheduler.Application.Slots;

namespace SchoolScheduler.Application.Slots.Queries.GetAllLectureSlots;

public record GetAllLectureSlotsQuery(Guid SchoolId) : IRequest<List<LectureSlotDto>>;

public class GetAllLectureSlotsQueryHandler : IRequestHandler<GetAllLectureSlotsQuery, List<LectureSlotDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAllLectureSlotsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<LectureSlotDto>> Handle(GetAllLectureSlotsQuery request, CancellationToken cancellationToken)
    {
        return await _context.LectureSlots
            .Where(s => s.SchoolId == request.SchoolId)
            .OrderBy(s => s.DayOfWeek)
            .ThenBy(s => s.SlotNumber)
            .Select(s => new LectureSlotDto(s.Id, s.SchoolId, s.DayOfWeek, s.SlotNumber, s.StartTime, s.EndTime))
            .ToListAsync(cancellationToken);
    }
}
