using System;
using SchoolScheduler.Domain.Entities;

namespace SchoolScheduler.Domain.Entities;

public class ClassSubjectAssignment
{
    public Guid Id { get; private set; }
    public Guid SchoolId { get; private set; }
    public Guid ClassRoomId { get; private set; }
    public Guid SubjectId { get; private set; }
    public Guid TeacherId { get; private set; }
    public int WeeklyQuota { get; private set; }

    public virtual ClassRoom ClassRoom { get; private set; } = null!;
    public virtual Subject Subject { get; private set; } = null!;
    public virtual Teacher Teacher { get; private set; } = null!;

    public ClassSubjectAssignment(Guid id, Guid schoolId, Guid classRoomId, Guid subjectId, Guid teacherId, int weeklyQuota)
    {
        if (weeklyQuota < 1) throw new ArgumentException("Weekly quota must be greater than zero.");

        Id = id;
        SchoolId = schoolId;
        ClassRoomId = classRoomId;
        SubjectId = subjectId;
        TeacherId = teacherId;
        WeeklyQuota = weeklyQuota;
    }

    public void Update(Guid teacherId, Guid subjectId, int weeklyQuota)
    {
        if (weeklyQuota < 1) throw new ArgumentException("Weekly quota must be greater than zero.");
        TeacherId = teacherId;
        SubjectId = subjectId;
        WeeklyQuota = weeklyQuota;
    }
}
