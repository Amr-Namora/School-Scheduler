using FluentValidation;
using SchoolScheduler.Application.Common.Interfaces;
using SchoolScheduler.Application.Assignments.Commands.CreateClassSubjectAssignment;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolScheduler.Application.Assignments.Commands.BulkCreateClassSubjectAssignment;

public class BulkCreateClassSubjectAssignmentCommandValidator : AbstractValidator<BulkCreateClassSubjectAssignmentCommand>
{
    private readonly IApplicationDbContext _context;

    public BulkCreateClassSubjectAssignmentCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        // Basic validation for each item in the list
        RuleForEach(x => x.Assignments).ChildRules(assignment =>
        {
            assignment.RuleFor(a => a.ClassRoomId).NotEmpty();
            assignment.RuleFor(a => a.SubjectId).NotEmpty();
            assignment.RuleFor(a => a.TeacherId).NotEmpty();
            assignment.RuleFor(a => a.WeeklyQuota).GreaterThan(0);
        });

        // Complex business rules validation to avoid N+1
        RuleFor(x => x).CustomAsync(async (cmd, context, cancellation) =>
        {
            var assignments = cmd.Assignments;
            if (assignments == null || !assignments.Any()) return;

            var classRoomIds = assignments.Select(a => a.ClassRoomId).Distinct().ToList();
            var teacherIds = assignments.Select(a => a.TeacherId).Distinct().ToList();
            var subjectIds = assignments.Select(a => a.SubjectId).Distinct().ToList();

            // Bulk fetch all necessary data using _context instead of context (ValidationContext)
            var classRooms = await _context.ClassRooms
                .Where(c => classRoomIds.Contains(c.Id))
                .Select(c => c.Id)
                .ToListAsync(cancellation);

            var teacherSubjects = await _context.TeacherSubjects
                .Where(ts => teacherIds.Contains(ts.TeacherId) && subjectIds.Contains(ts.SubjectId))
                .Select(ts => new { ts.TeacherId, ts.SubjectId })
                .ToListAsync(cancellation);

            var existingAssignments = await _context.ClassSubjectAssignments
                .Where(a => classRoomIds.Contains(a.ClassRoomId) || teacherIds.Contains(a.TeacherId))
                .ToListAsync(cancellation);

            var teacherAvailabilities = await _context.TeacherAvailabilities
                .Where(a => teacherIds.Contains(a.TeacherId))
                .ToListAsync(cancellation);

            // Track quotas and assignments within the bulk request itself
            var teacherCurrentQuotas = teacherIds.ToDictionary(
                id => id,
                id => existingAssignments.Where(a => a.TeacherId == id).Sum(a => a.WeeklyQuota)
            );

            var classRoomSubjectPairs = new HashSet<(Guid ClassRoomId, Guid SubjectId)>(
                existingAssignments.Select(a => (a.ClassRoomId, a.SubjectId))
            );

            for (int i = 0; i < assignments.Count; i++)
            {
                var item = assignments[i];

                // 1. ClassRoom existence
                if (!classRooms.Contains(item.ClassRoomId))
                {
                    context.AddFailure($"Assignments[{i}].ClassRoomId", "Class room not found.");
                }

                // 2. Teacher-Subject mapping
                if (!teacherSubjects.Any(ts => ts.TeacherId == item.TeacherId && ts.SubjectId == item.SubjectId))
                {
                    context.AddFailure($"Assignments[{i}].TeacherId", "This teacher is not assigned to teach this subject.");
                }

                // 3. Subject uniqueness per classroom
                if (classRoomSubjectPairs.Contains((item.ClassRoomId, item.SubjectId)))
                {
                    context.AddFailure($"Assignments[{i}].SubjectId", "This subject is already assigned to this classroom.");
                }
                classRoomSubjectPairs.Add((item.ClassRoomId, item.SubjectId));

                // 4. Teacher availability quota
                var availableSlotsCount = teacherAvailabilities
                    .Where(a => a.TeacherId == item.TeacherId)
                    .Count();

                if (teacherCurrentQuotas.ContainsKey(item.TeacherId))
                {
                    if (teacherCurrentQuotas[item.TeacherId] + item.WeeklyQuota > availableSlotsCount)
                    {
                        context.AddFailure($"Assignments[{i}].WeeklyQuota", "The requested quota exceeds the teacher's total weekly availability.");
                    }
                    teacherCurrentQuotas[item.TeacherId] += item.WeeklyQuota;
                }
                else
                {
                    context.AddFailure($"Assignments[{i}].TeacherId", "Teacher availability not found.");
                }
            }
        });
    }
}
