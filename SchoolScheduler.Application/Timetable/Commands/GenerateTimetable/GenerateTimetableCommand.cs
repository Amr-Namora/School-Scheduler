using MediatR;
using SchoolScheduler.Application.Common;
using SchoolScheduler.Application.Common.Interfaces;
using SchoolScheduler.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolScheduler.Application.Timetable.Commands.GenerateTimetable;

public record GenerateTimetableCommand(Guid SchoolId, Guid? ClassRoomId = null) : IRequest<bool>;

public class GenerateTimetableCommandHandler : IRequestHandler<GenerateTimetableCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ITimetableGenerationService _solver;

    public GenerateTimetableCommandHandler(IApplicationDbContext context, ITimetableGenerationService solver)
    {
        _context = context;
        _solver = solver;
    }

    public async Task<bool> Handle(GenerateTimetableCommand request, CancellationToken cancellationToken)
    {
        TimetableGenerationResult result;

        if (request.ClassRoomId.HasValue)
        {
            result = await _solver.GenerateForClassRoomAsync(request.ClassRoomId.Value, cancellationToken);
        }
        else
        {
            result = await _solver.GenerateAsync(request.SchoolId, cancellationToken);
        }

        if (!result.Success)
        {
            // In a real API, we might want to throw a custom exception with the failure reason
            // so the Controller can return a 400/422 with the details.
            throw new InvalidOperationException($"Timetable generation failed: {result.FailureReason} (Category: {result.ConstraintCategory})");
        }

        // Clear existing entries for the affected areas
        if (request.ClassRoomId.HasValue)
        {
            var existing = await _context.TimetableEntries
                .Where(e => e.ClassRoomId == request.ClassRoomId.Value)
                .ToListAsync(cancellationToken);

            _context.TimetableEntries.RemoveRange(existing);
        }
        else
        {
            // For whole school, we clear everything associated with that school
            var existing = await _context.TimetableEntries
                .Where(e => e.SchoolId == request.SchoolId)
                .ToListAsync(cancellationToken);

            _context.TimetableEntries.RemoveRange(existing);
        }

        foreach (var entry in result.Entries)
        {
            _context.TimetableEntries.Add(entry);
        }

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
