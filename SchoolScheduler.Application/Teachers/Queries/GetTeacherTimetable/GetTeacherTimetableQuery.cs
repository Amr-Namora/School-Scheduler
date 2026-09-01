using MediatR;
using SchoolScheduler.Application.Common.Interfaces;
using SchoolScheduler.Domain.Entities;
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
        var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.Id == request.TeacherId, cancellationToken);
        if (teacher == null) throw new Exception("Teacher not found.");

        var entries = await _context.TimetableEntries
            .Where(e => e.TeacherId == request.TeacherId)
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
                    _context.ClassRooms.FirstOrDefault(c => c.Id == e.ClassRoomId)?.Name,
                    _context.Subjects.FirstOrDefault(s => s.Id == e.SubjectId)?.Name,
                    _context.Schools.FirstOrDefault(s => s.Id == _context.ClassRooms.FirstOrDefault(c => c.Id == e.ClassRoomId)?.SchoolId)?.Name,
                    e.Status
                )).ToList();

            dayGrids.Add(new DayGridDto(day.DayOfWeek, daySlots));
        }

        return new TeacherTimetableDto(teacher.Id, teacher.Name, dayGrids);
    }
}
