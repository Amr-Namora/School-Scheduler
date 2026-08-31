using System;
using SchoolScheduler.Domain.Enums;

namespace SchoolScheduler.Domain.Entities;

public class LectureSlot
{
    public Guid Id { get; private set; }
    public Guid SchoolId { get; private set; }
    public SchoolDayOfWeek DayOfWeek { get; private set; }
    public int SlotNumber { get; private set; }
    public TimeSpan StartTime { get; private set; }
    public TimeSpan EndTime { get; private set; }

    public LectureSlot(Guid id, Guid schoolId, SchoolDayOfWeek dayOfWeek, int slotNumber, TimeSpan startTime, TimeSpan endTime)
    {
        if (slotNumber < 1) throw new ArgumentException("Slot number must be positive.");
        if (startTime >= endTime) throw new ArgumentException("Start time must be before end time.");

        Id = id;
        SchoolId = schoolId;
        DayOfWeek = dayOfWeek;
        SlotNumber = slotNumber;
        StartTime = startTime;
        EndTime = endTime;
    }
}
