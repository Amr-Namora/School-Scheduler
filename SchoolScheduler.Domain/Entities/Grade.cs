using System;
using SchoolScheduler.Domain.Enums;

namespace SchoolScheduler.Domain.Entities;

public class Grade
{
    public Guid Id { get; private set; }
    public Guid SchoolId { get; private set; }
    public GradeCategory Category { get; private set; }
    public int Level { get; private set; }

    public Grade(Guid id, Guid schoolId, GradeCategory category, int level)
    {
        if (level < 1) throw new ArgumentException("Grade level must be positive.");

        Id = id;
        SchoolId = schoolId;
        Category = category;
        Level = level;
    }

    public void Update(GradeCategory category, int level)
    {
        if (level < 1) throw new ArgumentException("Grade level must be positive.");

        Category = category;
        Level = level;
    }
}
