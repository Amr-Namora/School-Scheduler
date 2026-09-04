using System;
using SchoolScheduler.Domain.Common.Exceptions;

namespace SchoolScheduler.Domain.Entities;

public class Teacher
{
    public Guid Id { get; private set; }
    public Guid SchoolId { get; private set; }
    public string DisplayName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string? UserId { get; private set; }

    public Teacher(Guid id, Guid schoolId, string displayName, string email)
    {
        if (string.IsNullOrWhiteSpace(displayName)) throw new ArgumentException("Teacher display name is required.");
        if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Teacher email is required.");

        Id = id;
        SchoolId = schoolId;
        DisplayName = displayName;
        Email = email;
    }

    public void Update(string displayName, string email)
    {
        if (string.IsNullOrWhiteSpace(displayName)) throw new ArgumentException("Teacher display name is required.");
        if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Teacher email is required.");
        DisplayName = displayName;
        Email = email;
    }

    public void LinkToUser(string userId)
    {
        if (UserId != null)
        {
            throw new ConflictException("Teacher is already linked to a user.");
        }

        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new ArgumentException("User ID cannot be empty.");
        }

        UserId = userId;
    }
}
