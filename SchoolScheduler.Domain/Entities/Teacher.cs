using System;

namespace SchoolScheduler.Domain.Entities;

public class Teacher
{
    public Guid Id { get; private set; }
    public Guid SchoolId { get; private set; }
    public string Name { get; private set; } = string.Empty;

    public Teacher(Guid id, Guid schoolId, string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Teacher name is required.");

        Id = id;
        SchoolId = schoolId;
        Name = name;
    }
}
