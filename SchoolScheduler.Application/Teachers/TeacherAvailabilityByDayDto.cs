using System.Collections.Generic;
using SchoolScheduler.Domain.Enums;

namespace SchoolScheduler.Application.Teachers;

public record TeacherAvailabilityByDayDto(
    SchoolDayOfWeek DayOfWeek,
    List<int> SlotNumbers
);
