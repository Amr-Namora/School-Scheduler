using System;
using SchoolScheduler.Domain.Enums;

namespace SchoolScheduler.Domain.Entities;

public class TeacherAvailability
{
    public Guid Id { get; private set; }
    public Guid TeacherId { get; private set; }
    public SchoolDayOfWeek DayOfWeek { get; private set; }
    public int SlotNumber { get; private set; }

    public virtual Teacher Teacher { get; private set; } = null!;

    public TeacherAvailability(Guid id, Guid teacherId, SchoolDayOfWeek dayOfWeek, int slotNumber)
    {
        if (slotNumber < 1) throw new ArgumentException("Slot number must be positive.");

        Id = id;
        TeacherId = teacherId;
        DayOfWeek = dayOfWeek;
        SlotNumber = slotNumber;
    }
}
