using System;
using System.Collections.Generic;

namespace SchoolScheduler.Application.Teachers.Dtos;

public record TeacherConfigDto(
    Guid Id,
    string DisplayName,
    string Email,
    List<SubjectConfigDto> Subjects,
    List<AvailabilityConfigDto> Availability
);

public record SubjectConfigDto(Guid Id, string Name);

public record AvailabilityConfigDto(int DayOfWeek, int SlotNumber);
