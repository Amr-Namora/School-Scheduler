using MediatR;
using SchoolScheduler.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
using SchoolScheduler.Application.Slots;

namespace SchoolScheduler.Application.Slots.Queries.GetLectureSlotById;

public record GetLectureSlotByIdQuery(Guid Id) : IRequest<LectureSlotDto>;

public class GetLectureSlotByIdQueryHandler : IRequestHandler<GetLectureSlotByIdQuery, LectureSlotDto>
{
    private readonly IApplicationDbContext _context;

    public GetLectureSlotByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<LectureSlotDto> Handle(GetLectureSlotByIdQuery request, CancellationToken cancellationToken)
    {
        var slot = await _context.LectureSlots
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

        if (slot == null)
        {
            return null;
        }

        return new LectureSlotDto(slot.Id, slot.SchoolId, slot.DayOfWeek, slot.SlotNumber, slot.StartTime, slot.EndTime);
    }
}
