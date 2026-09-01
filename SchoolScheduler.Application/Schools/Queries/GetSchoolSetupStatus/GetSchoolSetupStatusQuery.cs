using MediatR;
using SchoolScheduler.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolScheduler.Application.Schools.Queries.GetSchoolSetupStatus;

public record SchoolSetupStatusDto(Guid SchoolId, List<SetupStepDto> Steps, bool IsReadyForGeneration);
public record SetupStepDto(string StepName, bool IsComplete, string? MissingInfo);

public record GetSchoolSetupStatusQuery(Guid SchoolId) : IRequest<SchoolSetupStatusDto>;

public class GetSchoolSetupStatusQueryHandler : IRequestHandler<GetSchoolSetupStatusQuery, SchoolSetupStatusDto>
{
    private readonly IApplicationDbContext _context;

    public GetSchoolSetupStatusQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SchoolSetupStatusDto> Handle(GetSchoolSetupStatusQuery request, CancellationToken cancellationToken)
    {
        var steps = new List<SetupStepDto>();

        // 1. Work Week Configuration
        var hasWorkingDays = await _context.SchoolWorkingDays.AnyAsync(d => d.SchoolId == request.SchoolId, cancellationToken);
        var hasSlots = hasWorkingDays && await _context.LectureSlots.AnyAsync(s => s.SchoolId == request.SchoolId, cancellationToken);
        steps.Add(new SetupStepDto("Configure Work Week", hasSlots, hasSlots ? null : "Define working days and lecture slots."));

        // 2. Grades and Classrooms
        var hasGrades = await _context.Grades.AnyAsync(g => g.SchoolId == request.SchoolId, cancellationToken);
        var hasRooms = await _context.ClassRooms.AnyAsync(c => c.SchoolId == request.SchoolId, cancellationToken);
        steps.Add(new SetupStepDto("Create Grades and Classrooms", hasGrades && hasRooms, (hasGrades && hasRooms) ? null : "Define grades and their corresponding classrooms."));

        // 3. Teachers and Subjects
        var hasTeachers = await _context.Teachers.AnyAsync(t => t.SchoolId == request.SchoolId, cancellationToken);
        var hasSubjects = await _context.Subjects.AnyAsync(s => s.SchoolId == request.SchoolId, cancellationToken);
        steps.Add(new SetupStepDto("Create Teachers and Subjects", hasTeachers && hasSubjects, (hasTeachers && hasSubjects) ? null : "Define teachers and subjects."));

        // 4. Class-Subject-Teacher Assignments
        var hasAssignments = await _context.ClassSubjectAssignments
            .AnyAsync(a => _context.ClassRooms.Any(c => c.Id == a.ClassRoomId && c.SchoolId == request.SchoolId), cancellationToken);
        steps.Add(new SetupStepDto("Configure Class-Subject-Teacher Assignments", hasAssignments, hasAssignments ? null : "Assign teachers to subjects for each classroom."));

        // 5. Teacher Availability
        var hasAvailability = await _context.TeacherAvailabilities
            .AnyAsync(a => _context.Teachers.Any(t => t.Id == a.TeacherId && t.SchoolId == request.SchoolId), cancellationToken);
        steps.Add(new SetupStepDto("Set Teacher Availability", hasAvailability, hasAvailability ? null : "Define when teachers are available to teach."));

        var isReady = steps.All(s => s.IsComplete);

        return new SchoolSetupStatusDto(request.SchoolId, steps, isReady);
    }
}
