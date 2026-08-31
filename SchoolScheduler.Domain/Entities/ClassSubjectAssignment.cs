using System;

namespace SchoolScheduler.Domain.Entities;

public class ClassSubjectAssignment
{
    public Guid Id { get; private set; }
    public Guid ClassRoomId { get; private set; }
    public Guid SubjectId { get; private set; }
    public Guid TeacherId { get; private set; }
    public int WeeklyQuota { get; private set; }

    public ClassSubjectAssignment(Guid id, Guid classRoomId, Guid subjectId, Guid teacherId, int weeklyQuota)
    {
        if (weeklyQuota < 1) throw new ArgumentException("Weekly quota must be greater than zero.");

        Id = id;
        ClassRoomId = classRoomId;
        SubjectId = subjectId;
        TeacherId = teacherId;
        WeeklyQuota = weeklyQuota;
    }
}
