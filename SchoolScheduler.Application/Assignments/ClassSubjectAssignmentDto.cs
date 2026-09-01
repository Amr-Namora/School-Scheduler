using System;

namespace SchoolScheduler.Application.Assignments;

public record ClassSubjectAssignmentDto(
    Guid Id,
    Guid ClassRoomId,
    Guid SubjectId,
    Guid TeacherId,
    int WeeklyQuota
);
