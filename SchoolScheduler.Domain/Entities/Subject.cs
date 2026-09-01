using System;

namespace SchoolScheduler.Domain.Entities;

public class Subject
{
    public Guid Id { get; private set; }
    public Guid SchoolId { get; private set; }
    public string Name { get; private set; } = string.Empty;

    public Subject(Guid id, Guid schoolId, string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Subject name is required.");

        Id = id;
        SchoolId = schoolId;
        Name = name;
    }

    public void Update(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Subject name is required.");
        Name = name;
    }
}
