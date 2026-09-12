using System;

namespace SchoolScheduler.Application.Teachers;

public record TeacherDto(Guid Id, Guid SchoolId, string Name, string Email);
