using System;
using SchoolScheduler.Domain.Enums;

namespace SchoolScheduler.Application.Teachers;

public record TeacherAvailabilityDto(Guid Id, Guid TeacherId, SchoolDayOfWeek DayOfWeek, int SlotNumber);
