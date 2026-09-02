using System;
using System.Collections.Generic;
using SchoolScheduler.Domain.Entities;

namespace SchoolScheduler.Infrastructure.Persistence.Solver;

public record SolverInput
{
    public Guid SchoolId { get; init; }
    public List<ClassRoom> ClassRooms { get; init; } = new();
    public List<Subject> Subjects { get; init; } = new();
    public List<Teacher> Teachers { get; init; } = new();
    public List<ClassSubjectAssignment> Assignments { get; init; } = new();
    public List<SchoolWorkingDay> WorkingDays { get; init; } = new();
    public List<LectureSlot> LectureSlots { get; init; } = new();
    public List<TeacherAvailability> Availabilities { get; init; } = new();
}

public record SolverOutput
{
    public bool Success { get; init; }
    public List<TimetableEntry> Entries { get; init; } = new();
    public string? FailureCategory { get; init; }
}
