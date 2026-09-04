using MediatR;
using SchoolScheduler.Domain.Common.Exceptions;
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

public record SlotDto(int SlotNumber, string? SubjectName, string? TeacherName, TimetableEntryStatus Status);
public record DayGridDto(SchoolDayOfWeek Day, List<SlotDto> Slots);
public record ClassTimetableDto(Guid ClassRoomId, string ClassRoomName, List<DayGridDto> Days);

public record GetClassTimetableQuery(Guid ClassRoomId, Guid SchoolId) : IRequest<ClassTimetableDto>;

public class GetClassTimetableQueryHandler : IRequestHandler<GetClassTimetableQuery, ClassTimetableDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IUserLookupService _userLookupService;

    public GetClassTimetableQueryHandler(IApplicationDbContext context, IUserLookupService userLookupService)
    {
        _context = context;
        _userLookupService = userLookupService;
    }

    public async Task<ClassTimetableDto> Handle(GetClassTimetableQuery request, CancellationToken cancellationToken)
    {
        var classRoom = await _context.ClassRooms
            .FirstOrDefaultAsync(c => c.Id == request.ClassRoomId && c.SchoolId == request.SchoolId, cancellationToken);

        if (classRoom == null)
        {
            throw new NotFoundException("Class room not found.");
        }

        var entries = await _context.TimetableEntries
            .Where(e => e.ClassRoomId == request.ClassRoomId)
            .ToListAsync(cancellationToken);

        var days = await _context.SchoolWorkingDays
            .Where(d => d.SchoolId == request.SchoolId)
            .OrderBy(d => (int)d.DayOfWeek)
            .ToListAsync(cancellationToken);

        var dayGrids = new List<DayGridDto>();

        foreach (var day in days)
        {
            var daySlots = entries
                .Where(e => e.DayOfWeek == day.DayOfWeek)
                .OrderBy(e => e.SlotNumber)
                .Select(async e =>
                {
                    var teacher = await _context.Teachers
                        .FirstOrDefaultAsync(t => t.Id == e.TeacherId, cancellationToken);

                    string? effectiveName = null;
                    if (teacher != null)
                    {
                        effectiveName = !string.IsNullOrEmpty(teacher.UserId)
                            ? await _userLookupService.GetFullNameAsync(teacher.UserId)
                            : teacher.DisplayName;
                    }

                    var subject = await _context.Subjects
                        .FirstOrDefaultAsync(s => s.Id == e.SubjectId, cancellationToken);

                    return new SlotDto(
                        e.SlotNumber,
                        subject?.Name,
                        effectiveName,
                        e.Status
                    );
                }).ToList();

            // Resolve the async tasks for the slots
            var resolvedSlots = await Task.WhenAll(daySlots);
            dayGrids.Add(new DayGridDto(day.DayOfWeek, resolvedSlots.ToList()));
        }

        return new ClassTimetableDto(classRoom.Id, classRoom.Name, dayGrids);
    }
}
