using System;
using SchoolScheduler.Domain.Enums;

namespace SchoolScheduler.Domain.Entities;

public class SchoolWorkingDay
{
    public Guid Id { get; private set; }
    public Guid SchoolId { get; private set; }
    public SchoolDayOfWeek DayOfWeek { get; private set; }

    public SchoolWorkingDay(Guid id, Guid schoolId, SchoolDayOfWeek dayOfWeek)
    {
        Id = id;
        SchoolId = schoolId;
        DayOfWeek = dayOfWeek;
    }
}
