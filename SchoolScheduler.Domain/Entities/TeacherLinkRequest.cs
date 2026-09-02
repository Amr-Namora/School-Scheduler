using SchoolScheduler.Domain.Enums;

namespace SchoolScheduler.Domain.Entities;

public class TeacherLinkRequest
{
    public Guid Id { get; private set; }
    public Guid TeacherId { get; private set; }
    public string UserId { get; private set; }
    public LinkRequestStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? RespondedAt { get; private set; }

    public TeacherLinkRequest(Guid teacherId, string userId)
    {
        Id = Guid.NewGuid();
        TeacherId = teacherId;
        UserId = userId;
        Status = LinkRequestStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    public void Respond(LinkRequestStatus status)
    {
        Status = status;
        RespondedAt = DateTime.UtcNow;
    }
}
