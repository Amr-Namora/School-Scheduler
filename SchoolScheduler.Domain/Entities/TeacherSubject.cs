using System;

namespace SchoolScheduler.Domain.Entities;

public record TeacherSubject(Guid TeacherId, Guid SubjectId);
