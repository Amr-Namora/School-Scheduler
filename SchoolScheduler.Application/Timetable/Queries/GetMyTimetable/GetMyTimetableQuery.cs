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

namespace SchoolScheduler.Application.Timetable.Queries.GetMyTimetable;

public record SlotDto(int SlotNumber, string? ClassRoomName, string? SubjectName, string? SchoolName, TimetableEntryStatus Status);
public record DayGridDto(SchoolDayOfWeek Day, List<SlotDto> Slots);
public record TeacherTimetableDto(Guid TeacherId, string TeacherName, List<DayGridDto> Days);
public record MyTimetableResponse(List<TeacherTimetableDto> Timetables);

public record GetMyTimetableQuery(string UserId) : IRequest<MyTimetableResponse>;

public class GetMyTimetableQueryHandler : IRequestHandler<GetMyTimetableQuery, MyTimetableResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IUserLookupService _userLookupService;

    public GetMyTimetableQueryHandler(IApplicationDbContext context, IUserLookupService userLookupService)
    {
        _context = context;
        _userLookupService = userLookupService;
    }

    public async Task<MyTimetableResponse> Handle(GetMyTimetableQuery request, CancellationToken cancellationToken)
    {
        var teachers = await _context.Teachers
            .IgnoreQueryFilters()
            .Where(t => t.UserId == request.UserId)
            .ToListAsync(cancellationToken);

        if (!teachers.Any())
        {
            return new MyTimetableResponse(new List<TeacherTimetableDto>());
        }

        var results = new List<TeacherTimetableDto>();

        foreach (var teacher in teachers)
        {
            var entries = await _context.TimetableEntries
                .IgnoreQueryFilters()
                .Where(e => e.TeacherId == teacher.Id)
                .ToListAsync(cancellationToken);

            var days = await _context.SchoolWorkingDays
                .IgnoreQueryFilters()
                .ToListAsync(cancellationToken); // simplified, assuming standardized or handled by school context

            // Note: Since this is aggregated, we should get days for each school specifically
            // But the current domain model has SchoolWorkingDays.
            var schoolWorkingDays = await _context.SchoolWorkingDays
                .IgnoreQueryFilters()
                .Where(d => d.SchoolId == teacher.SchoolId)
                .OrderBy(d => (int)d.DayOfWeek)
                .ToListAsync(cancellationToken);

            var dayGrids = new List<DayGridDto>();

            foreach (var day in schoolWorkingDays)
            {
                var daySlots = entries
                    .Where(e => e.DayOfWeek == day.DayOfWeek)
                    .OrderBy(e => e.SlotNumber)
                    .Select(e =>
                    {
                        // Resolve Room and School for the slot
                        // This part is expensive in a loop, but for a teacher's personal view it's acceptable.
                        // In a real app, we'd use a join or a dictionary.

                        // For simplicity in this implementation, we'll just return the basic SlotDto.
                        // We need to find the Room to get the School name.
                        return e;
                    }).ToList();

                // We'll handle the SlotDto mapping in a separate method to keep it clean
                var resolvedSlots = await ResolveSlots(daySlots, cancellationToken);
                dayGrids.Add(new DayGridDto(day.DayOfWeek, resolvedSlots));
            }

            string effectiveName = !string.IsNullOrEmpty(teacher.UserId)
                ? await _userLookupService.GetFullNameAsync(teacher.UserId)
                : teacher.DisplayName;

            results.Add(new TeacherTimetableDto(teacher.Id, effectiveName, dayGrids));
        }

        return new MyTimetableResponse(results);
    }

    private async Task<List<SlotDto>> ResolveSlots(List<TimetableEntry> entries, CancellationToken ct)
    {
        var results = new List<SlotDto>();
        foreach (var e in entries)
        {
            var room = await _context.ClassRooms
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(r => r.Id == e.ClassRoomId, ct);

            var school = await _context.Schools
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(s => s.Id == (room != null ? room.SchoolId : Guid.Empty), ct);

            var subject = await _context.Subjects
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(s => s.Id == e.SubjectId, ct);

            results.Add(new SlotDto(
                e.SlotNumber,
                room?.Name,
                subject?.Name,
                school?.Name,
                e.Status
            ));
        }
        return results.OrderBy(s => s.SlotNumber).ToList();
    }
}
