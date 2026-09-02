using Microsoft.EntityFrameworkCore;
using SchoolScheduler.Application.Common;
using SchoolScheduler.Application.Common.Interfaces;
using SchoolScheduler.Domain.Entities;
using SchoolScheduler.Infrastructure.Persistence.Solver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolScheduler.Infrastructure.Persistence;

public class TimetableGenerationService : ITimetableGenerationService
{
    private readonly IApplicationDbContext _context;
    private readonly CpSatTimetableSolver _solver;

    public TimetableGenerationService(IApplicationDbContext context)
    {
        _context = context;
        _solver = new CpSatTimetableSolver();
    }

    public async Task<TimetableGenerationResult> GenerateAsync(Guid schoolId, CancellationToken cancellationToken)
    {
        var input = await GetSolverInputAsync(schoolId, cancellationToken);
        var output = _solver.Solve(input);

        if (!output.Success)
        {
            return TimetableGenerationResult.Failed(
                $"Could not find a feasible timetable for school {schoolId}.",
                output.FailureCategory ?? "Unknown");
        }

        return TimetableGenerationResult.Succeeded(output.Entries);
    }

    public async Task<TimetableGenerationResult> GenerateForClassRoomAsync(Guid classRoomId, CancellationToken cancellationToken)
    {
        var classRoom = await _context.ClassRooms
            .FirstOrDefaultAsync(c => c.Id == classRoomId, cancellationToken);

        if (classRoom == null)
        {
            return TimetableGenerationResult.Failed("Class room not found.", "Input Validation");
        }

        var schoolId = classRoom.SchoolId;
        var input = await GetSolverInputAsync(schoolId, cancellationToken);

        // Filter input to only include the target classroom
        input = input with
        {
            ClassRooms = input.ClassRooms.Where(c => c.Id == classRoomId).ToList()
        };

        var output = _solver.Solve(input);

        if (!output.Success)
        {
            return TimetableGenerationResult.Failed(
                $"Could not find a feasible timetable for class room {classRoomId}.",
                output.FailureCategory ?? "Unknown");
        }

        return TimetableGenerationResult.Succeeded(output.Entries);
    }

    private async Task<SolverInput> GetSolverInputAsync(Guid schoolId, CancellationToken cancellationToken)
    {
        var classrooms = await _context.ClassRooms
            .Where(c => c.SchoolId == schoolId)
            .ToListAsync(cancellationToken);

        var subjects = await _context.Subjects
            .Where(s => s.SchoolId == schoolId)
            .ToListAsync(cancellationToken);

        var teachers = await _context.Teachers
            .Where(t => t.SchoolId == schoolId)
            .ToListAsync(cancellationToken);

        var assignments = await _context.ClassSubjectAssignments
            .Where(a => a.SchoolId == schoolId)
            .ToListAsync(cancellationToken);

        var workingDays = await _context.SchoolWorkingDays
            .Where(wd => wd.SchoolId == schoolId)
            .ToListAsync(cancellationToken);

        var lectureSlots = await _context.LectureSlots
            .Where(ls => ls.SchoolId == schoolId)
            .ToListAsync(cancellationToken);

        var availabilities = await _context.TeacherAvailabilities
            .Where(ta => ta.Teacher.SchoolId == schoolId)
            .ToListAsync(cancellationToken);

        return new SolverInput
        {
            SchoolId = schoolId,
            ClassRooms = classrooms,
            Subjects = subjects,
            Teachers = teachers,
            Assignments = assignments,
            WorkingDays = workingDays,
            LectureSlots = lectureSlots,
            Availabilities = availabilities
        };
    }
}
