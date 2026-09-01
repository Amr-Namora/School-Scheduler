using Microsoft.EntityFrameworkCore;
using SchoolScheduler.Domain.Entities;
using System;

namespace SchoolScheduler.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<School> Schools { get; }
    DbSet<Grade> Grades { get; }
    DbSet<ClassRoom> ClassRooms { get; }
    DbSet<Subject> Subjects { get; }
    DbSet<Teacher> Teachers { get; }
    DbSet<TeacherSubject> TeacherSubjects { get; }
    DbSet<ClassSubjectAssignment> ClassSubjectAssignments { get; }
    DbSet<SchoolWorkingDay> SchoolWorkingDays { get; }
    DbSet<LectureSlot> LectureSlots { get; }
    DbSet<BreakSlot> BreakSlots { get; }
    DbSet<TeacherAvailability> TeacherAvailabilities { get; }
    DbSet<TimetableEntry> TimetableEntries { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
