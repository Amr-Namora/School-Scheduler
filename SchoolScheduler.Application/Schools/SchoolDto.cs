using System;

namespace SchoolScheduler.Application.Schools;

public record SchoolDto(Guid Id, string Name, int LecturesPerDay);
