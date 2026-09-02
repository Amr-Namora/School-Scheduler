using System;
using System.Collections.Generic;
using SchoolScheduler.Domain.Entities;

namespace SchoolScheduler.Application.Common;

public record TimetableGenerationResult
{
    public bool Success { get; init; }
    public IEnumerable<TimetableEntry> Entries { get; init; } = new List<TimetableEntry>();
    public string? FailureReason { get; init; }
    public string? ConstraintCategory { get; init; }

    public static TimetableGenerationResult Failed(string reason, string category)
        => new() { Success = false, FailureReason = reason, ConstraintCategory = category };

    public static TimetableGenerationResult Succeeded(IEnumerable<TimetableEntry> entries)
        => new() { Success = true, Entries = entries };
}
