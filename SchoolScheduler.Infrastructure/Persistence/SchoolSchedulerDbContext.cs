using Microsoft.EntityFrameworkCore;
using SchoolScheduler.Application.Common.Interfaces;
using SchoolScheduler.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolScheduler.Infrastructure.Persistence;

public class SchoolSchedulerDbContext : DbContext, IApplicationDbContext
{
    private readonly ICurrentSchoolContext _currentSchoolContext;

    public SchoolSchedulerDbContext(DbContextOptions<SchoolSchedulerDbContext> options, ICurrentSchoolContext currentSchoolContext)
        : base(options)
    {
        _currentSchoolContext = currentSchoolContext;
    }

    public DbSet<School> Schools => Set<School>();
    public DbSet<Grade> Grades => Set<Grade>();
    public DbSet<ClassRoom> ClassRooms => Set<ClassRoom>();
    public DbSet<Subject> Subjects => Set<Subject>();
    public DbSet<Teacher> Teachers => Set<Teacher>();
    public DbSet<TeacherSubject> TeacherSubjects => Set<TeacherSubject>();
    public DbSet<ClassSubjectAssignment> ClassSubjectAssignments => Set<ClassSubjectAssignment>();
    public DbSet<SchoolWorkingDay> SchoolWorkingDays => Set<SchoolWorkingDay>();
    public DbSet<LectureSlot> LectureSlots => Set<LectureSlot>();
    public DbSet<BreakSlot> BreakSlots => Set<BreakSlot>();
    public DbSet<TeacherAvailability> TeacherAvailabilities => Set<TeacherAvailability>();
    public DbSet<TimetableEntry> TimetableEntries => Set<TimetableEntry>();
    public DbSet<TeacherLinkRequest> TeacherLinkRequests => Set<TeacherLinkRequest>();

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return await base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // School
        modelBuilder.Entity<School>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasQueryFilter(s => !s.IsDeleted);
        });

        // Grade
        modelBuilder.Entity<Grade>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne<School>()
                .WithMany()
                .HasForeignKey(e => e.SchoolId);
            entity.HasQueryFilter(e => e.SchoolId == _currentSchoolContext.SchoolId);
        });

        // ClassRoom
        modelBuilder.Entity<ClassRoom>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne<School>()
                .WithMany()
                .HasForeignKey(e => e.SchoolId);
            entity.HasOne<Grade>()
                .WithMany()
                .HasForeignKey(e => e.GradeId);
            entity.HasQueryFilter(e => e.SchoolId == _currentSchoolContext.SchoolId);
        });

        // Subject
        modelBuilder.Entity<Subject>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne<School>()
                .WithMany()
                .HasForeignKey(e => e.SchoolId);
            entity.HasQueryFilter(e => e.SchoolId == _currentSchoolContext.SchoolId);
        });

        // Teacher
        modelBuilder.Entity<Teacher>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne<School>()
                .WithMany()
                .HasForeignKey(e => e.SchoolId);
            entity.HasQueryFilter(e => e.SchoolId == _currentSchoolContext.SchoolId);
        });

        // TeacherSubject
        modelBuilder.Entity<TeacherSubject>(entity =>
        {
            entity.HasKey(e => new { e.TeacherId, e.SubjectId });
            entity.HasOne(e => e.Teacher)
                .WithMany()
                .HasForeignKey(e => e.TeacherId);
            entity.HasOne(e => e.Subject)
                .WithMany()
                .HasForeignKey(e => e.SubjectId);
        });

        // ClassSubjectAssignment
        modelBuilder.Entity<ClassSubjectAssignment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne<School>()
                .WithMany()
                .HasForeignKey(e => e.SchoolId);
            entity.HasOne(e => e.ClassRoom)
                .WithMany()
                .HasForeignKey(e => e.ClassRoomId);
            entity.HasOne(e => e.Subject)
                .WithMany()
                .HasForeignKey(e => e.SubjectId);
            entity.HasOne(e => e.Teacher)
                .WithMany()
                .HasForeignKey(e => e.TeacherId);
            entity.HasQueryFilter(e => e.SchoolId == _currentSchoolContext.SchoolId);
        });

        // SchoolWorkingDay
        modelBuilder.Entity<SchoolWorkingDay>(entity =>
        {
            entity.HasKey(e => new { e.SchoolId, e.DayOfWeek });
            entity.HasOne<School>()
                .WithMany()
                .HasForeignKey(e => e.SchoolId);
            entity.HasQueryFilter(e => e.SchoolId == _currentSchoolContext.SchoolId);
        });

        // LectureSlot
        modelBuilder.Entity<LectureSlot>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne<School>()
                .WithMany()
                .HasForeignKey(e => e.SchoolId);
            entity.HasQueryFilter(e => e.SchoolId == _currentSchoolContext.SchoolId);
        });

        // BreakSlot
        modelBuilder.Entity<BreakSlot>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne<School>()
                .WithMany()
                .HasForeignKey(e => e.SchoolId);
            entity.HasQueryFilter(e => e.SchoolId == _currentSchoolContext.SchoolId);
        });

        // TeacherAvailability
        modelBuilder.Entity<TeacherAvailability>(entity =>
        {
            entity.HasKey(e => new { e.TeacherId, e.DayOfWeek, e.SlotNumber });
            entity.HasOne(e => e.Teacher)
                .WithMany()
                .HasForeignKey(e => e.TeacherId);
        });

        // TimetableEntry
        modelBuilder.Entity<TimetableEntry>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne<School>()
                .WithMany()
                .HasForeignKey(e => e.SchoolId);
            entity.HasOne(e => e.ClassRoom)
                .WithMany()
                .HasForeignKey(e => e.ClassRoomId);
            entity.HasOne(e => e.Subject)
                .WithMany()
                .HasForeignKey(e => e.SubjectId);
            entity.HasOne(e => e.Teacher)
                .WithMany()
                .HasForeignKey(e => e.TeacherId);
            entity.HasQueryFilter(e => e.SchoolId == _currentSchoolContext.SchoolId);
        });

        // TeacherLinkRequest
        modelBuilder.Entity<TeacherLinkRequest>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Teacher)
                .WithMany()
                .HasForeignKey(e => e.TeacherId);
        });
    }
}
