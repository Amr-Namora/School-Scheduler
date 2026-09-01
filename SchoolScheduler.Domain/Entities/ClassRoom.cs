using System;

namespace SchoolScheduler.Domain.Entities;

public class ClassRoom
{
    public Guid Id { get; private set; }
    public Guid SchoolId { get; private set; }
    public Guid GradeId { get; private set; }
    public string Name { get; private set; } = string.Empty;

    public ClassRoom(Guid id, Guid schoolId, Guid gradeId, string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Class room name is required.");

        Id = id;
        SchoolId = schoolId;
        GradeId = gradeId;
        Name = name;
    }

    public void Update(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Class room name is required.");
        Name = name;
    }
}
