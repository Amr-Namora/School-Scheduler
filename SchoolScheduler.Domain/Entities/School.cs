using System;
using SchoolScheduler.Domain.Common.Exceptions;

namespace SchoolScheduler.Domain.Entities;

public class School
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string UserId { get; private set; } = string.Empty;
    public int LecturesPerDay { get; private set; }
    public bool IsDeleted { get; private set; } = false;
    public DateTime? DeletedAt { get; private set; }

    public School(Guid id, string name, string userId, int lecturesPerDay)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("School name is required.");
        if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("User ID is required.");
        if (lecturesPerDay < 1) throw new ArgumentException("Lectures per day must be at least 1.");

        Id = id;
        Name = name;
        UserId = userId;
        LecturesPerDay = lecturesPerDay;
    }

    public void Update(string name, int lecturesPerDay)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("School name is required.");
        if (lecturesPerDay < 1) throw new ArgumentException("Lectures per day must be at least 1.");

        Name = name;
        LecturesPerDay = lecturesPerDay;
    }

    public void SoftDelete()
    {
        if (IsDeleted)
        {
            throw new ConflictException("School is already deleted.");
        }

        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
    }
}
