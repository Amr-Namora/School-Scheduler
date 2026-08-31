using System;
using SchoolScheduler.Domain.Enums;

namespace SchoolScheduler.Domain.Entities;

public class BreakSlot
{
    public Guid Id { get; private set; }
    public Guid SchoolId { get; private set; }
    public SchoolDayOfWeek DayOfWeek { get; private set; }
    public int AfterSlotNumber { get; private set; }
    public TimeSpan StartTime { get; private set; }
    public TimeSpan EndTime { get; private set; }

    public BreakSlot(Guid id, Guid schoolId, SchoolDayOfWeek dayOfWeek, int afterSlotNumber, TimeSpan startTime, TimeSpan endTime)
    {
        if (afterSlotNumber < 1) throw new ArgumentException("AfterSlotNumber must be positive.");
        if (startTime >= endTime) throw new ArgumentException("Start time must be before end time.");

        Id = id;
        SchoolId = schoolId;
        DayOfWeek = dayOfWeek;
        AfterSlotNumber = afterSlotNumber;
        StartTime = startTime;
        EndTime = endTime;
    }
}
