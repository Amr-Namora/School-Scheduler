using System;

namespace SchoolScheduler.Application.Slots;

public record BreakSlotDto(Guid Id, Guid SchoolId, SchoolScheduler.Domain.Enums.SchoolDayOfWeek DayOfWeek, int AfterSlotNumber, TimeSpan StartTime, TimeSpan EndTime);
