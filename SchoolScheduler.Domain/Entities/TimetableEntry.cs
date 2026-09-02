using System;
using SchoolScheduler.Domain.Enums;

namespace SchoolScheduler.Domain.Entities;

public class TimetableEntry
{
    public Guid Id { get; private set; }
    public Guid SchoolId { get; private set; }
    public Guid ClassRoomId { get; private set; }
    public Guid? SubjectId { get; private set; }
    public Guid? TeacherId { get; private set; }
    public SchoolDayOfWeek DayOfWeek { get; private set; }
    public int SlotNumber { get; private set; }
    public TimetableEntryStatus Status { get; private set; }

    public virtual ClassRoom ClassRoom { get; private set; } = null!;
    public virtual Subject? Subject { get; private set; }
    public virtual Teacher? Teacher { get; private set; }

    private TimetableEntry(Guid id, Guid schoolId, Guid classRoomId, Guid? subjectId, Guid? teacherId, SchoolDayOfWeek dayOfWeek, int slotNumber, TimetableEntryStatus status)
    {
        if (slotNumber < 1) throw new ArgumentException("Slot number must be positive.");

        if (status == TimetableEntryStatus.Scheduled && (subjectId == null || teacherId == null))
            throw new ArgumentException("Scheduled entries must have both a Subject and a Teacher.");

        if (status == TimetableEntryStatus.Empty && (subjectId != null || teacherId != null))
            throw new ArgumentException("Empty entries must not have a Subject or a Teacher.");

        Id = id;
        SchoolId = schoolId;
        ClassRoomId = classRoomId;
        SubjectId = subjectId;
        TeacherId = teacherId;
        DayOfWeek = dayOfWeek;
        SlotNumber = slotNumber;
        Status = status;
    }

    public static TimetableEntry CreateScheduled(Guid id, Guid schoolId, Guid classRoomId, Guid subjectId, Guid teacherId, SchoolDayOfWeek dayOfWeek, int slotNumber)
    {
        return new TimetableEntry(id, schoolId, classRoomId, subjectId, teacherId, dayOfWeek, slotNumber, TimetableEntryStatus.Scheduled);
    }

    public static TimetableEntry CreateEmpty(Guid id, Guid schoolId, Guid classRoomId, SchoolDayOfWeek dayOfWeek, int slotNumber)
    {
        return new TimetableEntry(id, schoolId, classRoomId, null, null, dayOfWeek, slotNumber, TimetableEntryStatus.Empty);
    }
}
