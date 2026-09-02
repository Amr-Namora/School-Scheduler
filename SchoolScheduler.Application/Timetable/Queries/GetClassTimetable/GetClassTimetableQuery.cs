using MediatR;
using SchoolScheduler.Application.Common.Interfaces;
using SchoolScheduler.Domain.Entities;
using SchoolScheduler.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolScheduler.Application.Timetable.Queries.GetClassTimetable;

public record ClassTimetableDto(Guid ClassRoomId, List<DayGridDto> Days);
public record DayGridDto(SchoolDayOfWeek Day, List<SlotDto> Slots);
public record SlotDto(int SlotNumber, string? SubjectName, string? TeacherName, TimetableEntryStatus Status);

public record GetClassTimetableQuery(Guid ClassRoomId) : IRequest<ClassTimetableDto>;

public class GetClassTimetableQueryHandler : IRequestHandler<GetClassTimetableQuery, ClassTimetableDto>
{
    private readonly IApplicationDbContext _context;

    public GetClassTimetableQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ClassTimetableDto> Handle(GetClassTimetableQuery request, CancellationToken cancellationToken)
    {
        var entries = await _context.TimetableEntries
            .Where(e => e.ClassRoomId == request.ClassRoomId)
            .ToListAsync(cancellationToken);

        var days = await _context.SchoolWorkingDays
            .OrderBy(d => (int)d.DayOfWeek)
            .ToListAsync(cancellationToken);

        var dayGrids = new List<DayGridDto>();

        foreach (var day in days)
        {
            var daySlots = entries.Where(e => e.DayOfWeek == day.DayOfWeek)
                .OrderBy(e => e.SlotNumber)
                .Select(e => new SlotDto(
                    e.SlotNumber,
                    _context.Subjects.FirstOrDefault(s => s.Id == e.SubjectId)?.Name,
                    _context.Teachers.FirstOrDefault(t => t.Id == e.TeacherId)?.DisplayName,
                    e.Status
                )).ToList();

            dayGrids.Add(new DayGridDto(day.DayOfWeek, daySlots));
        }

        return new ClassTimetableDto(request.ClassRoomId, dayGrids);
    }
}
