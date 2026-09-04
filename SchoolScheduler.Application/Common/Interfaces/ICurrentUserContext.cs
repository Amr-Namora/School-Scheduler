namespace SchoolScheduler.Application.Common.Interfaces;

public interface ICurrentUserContext
{
    string? UserId { get; }
    string? Role { get; }
}
