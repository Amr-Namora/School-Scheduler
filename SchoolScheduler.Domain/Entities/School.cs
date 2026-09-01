using System;

namespace SchoolScheduler.Domain.Entities;

public class School
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public int LecturesPerDay { get; private set; }

    public School(Guid id, string name, int lecturesPerDay)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("School name is required.");
        if (lecturesPerDay < 1) throw new ArgumentException("Lectures per day must be at least 1.");

        Id = id;
        Name = name;
        LecturesPerDay = lecturesPerDay;
    }

    public void Update(string name, int lecturesPerDay)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("School name is required.");
        if (lecturesPerDay < 1) throw new ArgumentException("Lectures per day must be at least 1.");

        Name = name;
        LecturesPerDay = lecturesPerDay;
    }
}
