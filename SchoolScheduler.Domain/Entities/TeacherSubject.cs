using System;

namespace SchoolScheduler.Domain.Entities;

public class TeacherSubject
{
    public Guid TeacherId { get; private set; }
    public Guid SubjectId { get; private set; }

    public virtual Teacher Teacher { get; private set; } = null!;
    public virtual Subject Subject { get; private set; } = null!;

    public TeacherSubject(Guid teacherId, Guid subjectId)
    {
        TeacherId = teacherId;
        SubjectId = subjectId;
    }
}
