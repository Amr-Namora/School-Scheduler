using MediatR;
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
        IEnumerable<TimetableEntry> results;

        if (request.ClassRoomId.HasValue)
        {
            results = await _solver.GenerateForClassRoomAsync(request.ClassRoomId.Value, cancellationToken);
        }
        else
        {
            results = await _solver.GenerateAsync(request.SchoolId, cancellationToken);
        }

        if (results == null || !results.Any())
        {
            return false;
        }

        // Clear existing entries for the affected areas
        if (request.ClassRoomId.HasValue)
        {
            var existing = _context.TimetableEntries.Where(e => e.ClassRoomId == request.ClassRoomId.Value).ToList();
            foreach (var entry in existing) _context.TimetableEntries.Remove(entry);
        }
        else
        {
            // For whole school, we clear everything associated with that school's classrooms
            var classrooms = _context.ClassRooms.Where(c => c.SchoolId == request.SchoolId).Select(c => c.Id).ToList();
            var existing = _context.TimetableEntries.Where(e => classrooms.Contains(e.ClassRoomId)).ToList();
            foreach (var entry in existing) _context.TimetableEntries.Remove(entry);
        }

        foreach (var entry in results)
        {
            _context.TimetableEntries.Add(entry);
        }

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
