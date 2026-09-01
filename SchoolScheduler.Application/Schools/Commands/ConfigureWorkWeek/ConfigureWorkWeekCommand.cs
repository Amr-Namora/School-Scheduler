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
    List<SlotConfig> LectureSlots,
    List<BreakConfig> BreakSlots
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
        // Clear existing configuration for this school
        var existingDays = _context.SchoolWorkingDays.Where(d => d.SchoolId == request.SchoolId).ToList();
        foreach (var day in existingDays)
        {
            _context.SchoolWorkingDays.Remove(day);
        }

        // Clear existing slots
        var existingLectureSlots = _context.LectureSlots.Where(s => s.SchoolId == request.SchoolId).ToList();
        foreach (var slot in existingLectureSlots)
        {
            _context.LectureSlots.Remove(slot);
        }

        var existingBreakSlots = _context.BreakSlots.Where(b => b.SchoolId == request.SchoolId).ToList();
        foreach (var brk in existingBreakSlots)
        {
            _context.BreakSlots.Remove(brk);
        }

        // Add new configuration
        foreach (var dayConfig in request.DayConfigs)
        {
            var workingDay = new SchoolWorkingDay(Guid.NewGuid(), request.SchoolId, dayConfig.DayOfWeek);
            _context.SchoolWorkingDays.Add(workingDay);

            foreach (var slotConfig in dayConfig.LectureSlots)
            {
                var slot = new LectureSlot(Guid.NewGuid(), request.SchoolId, dayConfig.DayOfWeek, slotConfig.SlotNumber, slotConfig.StartTime, slotConfig.EndTime);
                _context.LectureSlots.Add(slot);
            }

            foreach (var breakConfig in dayConfig.BreakSlots)
            {
                var brk = new BreakSlot(Guid.NewGuid(), request.SchoolId, dayConfig.DayOfWeek, breakConfig.AfterSlotNumber, breakConfig.StartTime, breakConfig.EndTime);
                _context.BreakSlots.Add(brk);
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
