using MediatR;
using SchoolScheduler.Application.Common.Interfaces;
using SchoolScheduler.Domain.Entities;
using SchoolScheduler.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolScheduler.Application.Schools.Commands.ConfigureWorkWeek;

public record WorkWeekDayConfig(
    SchoolDayOfWeek DayOfWeek,
    List<SlotConfig>? LectureSlots,
    List<BreakConfig>? BreakSlots
);

public record SlotConfig(int SlotNumber, TimeSpan StartTime, TimeSpan EndTime);
public record BreakConfig(int AfterSlotNumber, TimeSpan StartTime, TimeSpan EndTime);

public record ConfigureWorkWeekCommand(
    Guid SchoolId,
    List<WorkWeekDayConfig> DayConfigs
) : IRequest<bool>;

public class ConfigureWorkWeekCommandHandler : IRequestHandler<ConfigureWorkWeekCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public ConfigureWorkWeekCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(ConfigureWorkWeekCommand request, CancellationToken cancellationToken)
    {
        // Remove days not present in the request
        var existingDays = _context.SchoolWorkingDays.Where(d => d.SchoolId == request.SchoolId).ToList();
        var requestedDays = request.DayConfigs.Select(d => d.DayOfWeek).ToList();

        var daysToRemove = existingDays.Where(d => !requestedDays.Contains(d.DayOfWeek)).ToList();
        foreach (var day in daysToRemove)
        {
            // Remove slots for the day being removed
            var slotsToRemove = _context.LectureSlots.Where(s => s.SchoolId == day.SchoolId && s.DayOfWeek == day.DayOfWeek).ToList();
            _context.LectureSlots.RemoveRange(slotsToRemove);

            var breaksToRemove = _context.BreakSlots.Where(b => b.SchoolId == day.SchoolId && b.DayOfWeek == day.DayOfWeek).ToList();
            _context.BreakSlots.RemoveRange(breaksToRemove);

            _context.SchoolWorkingDays.Remove(day);
        }

        // Reconcile each requested day
        foreach (var dayConfig in request.DayConfigs)
        {
            var workingDay = existingDays.FirstOrDefault(d => d.DayOfWeek == dayConfig.DayOfWeek);
            if (workingDay == null)
            {
                workingDay = new SchoolWorkingDay(Guid.NewGuid(), request.SchoolId, dayConfig.DayOfWeek);
                _context.SchoolWorkingDays.Add(workingDay);
            }

            // Reconcile Lecture Slots: if null, preserve; if empty or provided, replace
            if (dayConfig.LectureSlots != null)
            {
                var existingSlots = _context.LectureSlots.Where(s => s.SchoolId == request.SchoolId && s.DayOfWeek == dayConfig.DayOfWeek).ToList();
                _context.LectureSlots.RemoveRange(existingSlots);

                foreach (var slotConfig in dayConfig.LectureSlots)
                {
                    var slot = new LectureSlot(Guid.NewGuid(), request.SchoolId, dayConfig.DayOfWeek, slotConfig.SlotNumber, slotConfig.StartTime, slotConfig.EndTime);
                    _context.LectureSlots.Add(slot);
                }
            }

            // Reconcile Break Slots: if null, preserve; if empty or provided, replace
            if (dayConfig.BreakSlots != null)
            {
                var existingBreaks = _context.BreakSlots.Where(b => b.SchoolId == request.SchoolId && b.DayOfWeek == dayConfig.DayOfWeek).ToList();
                _context.BreakSlots.RemoveRange(existingBreaks);

                foreach (var breakConfig in dayConfig.BreakSlots)
                {
                    var brk = new BreakSlot(Guid.NewGuid(), request.SchoolId, dayConfig.DayOfWeek, breakConfig.AfterSlotNumber, breakConfig.StartTime, breakConfig.EndTime);
                    _context.BreakSlots.Add(brk);
                }
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
