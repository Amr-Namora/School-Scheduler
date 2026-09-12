using MediatR;
using SchoolScheduler.Application.Common.Interfaces;
using SchoolScheduler.Application.Assignments.Commands.CreateClassSubjectAssignment;
using SchoolScheduler.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolScheduler.Application.Assignments.Commands.BulkCreateClassSubjectAssignment;

public class BulkCreateClassSubjectAssignmentCommandHandler : IRequestHandler<BulkCreateClassSubjectAssignmentCommand, int>
{
    private readonly IApplicationDbContext _context;

    public BulkCreateClassSubjectAssignmentCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(BulkCreateClassSubjectAssignmentCommand request, CancellationToken cancellationToken)
    {
        // To create the ClassSubjectAssignment, we need the SchoolId from the ClassRoom.
        // We fetch all unique ClassRoomIds from the request to get their SchoolIds.
        var classRoomIds = request.Assignments.Select(a => a.ClassRoomId).Distinct().ToList();
        var classRooms = await _context.ClassRooms
            .Where(c => classRoomIds.Contains(c.Id))
            .ToDictionaryAsync(c => c.Id, c => c.SchoolId, cancellationToken);

        var assignmentsToAdd = new List<ClassSubjectAssignment>();

        foreach (var req in request.Assignments)
        {
            if (!classRooms.TryGetValue(req.ClassRoomId, out var schoolId))
            {
                // This should technically be caught by the validator, but we add a safety check.
                throw new Exception($"ClassRoom with ID {req.ClassRoomId} not found.");
            }

            var assignment = new ClassSubjectAssignment(
                Guid.NewGuid(),
                schoolId,
                req.ClassRoomId,
                req.SubjectId,
                req.TeacherId,
                req.WeeklyQuota);

            assignmentsToAdd.Add(assignment);
        }

        _context.ClassSubjectAssignments.AddRange(assignmentsToAdd);
        await _context.SaveChangesAsync(cancellationToken);

        return assignmentsToAdd.Count;
    }
}
