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

namespace SchoolScheduler.Application.Teachers.Queries.GetTeacherTimetable;

public record TeacherTimetableDto(Guid TeacherId, string TeacherName, List<DayGridDto> Days);
public record DayGridDto(SchoolDayOfWeek Day, List<SlotDto> Slots);
public record SlotDto(int SlotNumber, string? ClassRoomName, string? SubjectName, string? SchoolName, TimetableEntryStatus Status);

public record GetTeacherTimetableQuery(Guid TeacherId) : IRequest<TeacherTimetableDto>;

public class GetTeacherTimetableQueryHandler : IRequestHandler<GetTeacherTimetableQuery, TeacherTimetableDto>
{
    private readonly IApplicationDbContext _context;

    public GetTeacherTimetableQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<TeacherTimetableDto> Handle(GetTeacherTimetableQuery request, CancellationToken cancellationToken)
    {
        var teacher = await _context.Teachers
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(t => t.Id == request.TeacherId, cancellationToken);
        if (teacher == null) throw new Exception("Teacher not found.");

        var entries = await _context.TimetableEntries
            .IgnoreQueryFilters()
            .Where(e => e.TeacherId == request.TeacherId)
            .ToListAsync(cancellationToken);

        // ClassRoomId is a Guid. SubjectId is a Guid?. Treat them differently!
        var classRoomIds = entries.Select(e => e.ClassRoomId).Distinct().ToList();
        var subjectIds = entries.Where(e => e.SubjectId.HasValue).Select(e => e.SubjectId.Value).Distinct().ToList();

        var classRooms = await _context.ClassRooms
            .IgnoreQueryFilters()
            .Where(c => classRoomIds.Contains(c.Id))
            .ToDictionaryAsync(c => c.Id, cancellationToken);

        var schoolIds = classRooms.Values.Select(c => c.SchoolId).Distinct().ToList();
        var schools = await _context.Schools
            .IgnoreQueryFilters()
            .Where(s => schoolIds.Contains(s.Id))
            .ToDictionaryAsync(s => s.Id, cancellationToken);

        var subjects = await _context.Subjects
            .IgnoreQueryFilters()
            .Where(s => subjectIds.Contains(s.Id))
            .ToDictionaryAsync(s => s.Id, cancellationToken);

        var days = await _context.SchoolWorkingDays
            .IgnoreQueryFilters()
            .OrderBy(d => (int)d.DayOfWeek)
            .ToListAsync(cancellationToken);

        var dayGrids = new List<DayGridDto>();

        foreach (var day in days)
        {
            var daySlots = entries
                .Where(e => e.DayOfWeek == day.DayOfWeek)
                .OrderBy(e => e.SlotNumber)
                .Select(e =>
                {
                    // ClassRoom is non-nullable, lookup directly
                    classRooms.TryGetValue(e.ClassRoomId, out var room);

                    // Subject is nullable, check HasValue first
                    Subject subject = null;
                    if (e.SubjectId.HasValue)
                    {
                        subjects.TryGetValue(e.SubjectId.Value, out subject);
                    }

                    School school = null;
                    if (room != null)
                    {
                        schools.TryGetValue(room.SchoolId, out school);
                    }

                    return new SlotDto(
                        e.SlotNumber,
                        room?.Name,
                        subject?.Name,
                        school?.Name,
                        e.Status
                    );
                }).ToList();

            dayGrids.Add(new DayGridDto(day.DayOfWeek, daySlots));
        }

        return new TeacherTimetableDto(teacher.Id, teacher.DisplayName, dayGrids);
    }
}