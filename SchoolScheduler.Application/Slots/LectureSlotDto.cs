using System;

namespace SchoolScheduler.Application.Slots;

public record LectureSlotDto(Guid Id, Guid SchoolId, SchoolScheduler.Domain.Enums.SchoolDayOfWeek DayOfWeek, int SlotNumber, TimeSpan StartTime, TimeSpan EndTime);
