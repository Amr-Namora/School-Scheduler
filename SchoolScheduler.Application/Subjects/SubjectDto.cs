using System;

namespace SchoolScheduler.Application.Subjects;

public record SubjectDto(Guid Id, Guid SchoolId, string Name);
