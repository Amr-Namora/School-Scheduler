using System;
using SchoolScheduler.Domain.Enums;

namespace SchoolScheduler.Application.Grades;

public record GradeDto(Guid Id, Guid SchoolId, GradeCategory Category, int Level);
