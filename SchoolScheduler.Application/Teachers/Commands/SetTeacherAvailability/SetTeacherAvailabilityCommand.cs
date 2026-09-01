using MediatR;
using SchoolScheduler.Application.Common.Interfaces;
using SchoolScheduler.Domain.Entities;
using SchoolScheduler.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolScheduler.Application.Teachers.Commands.SetTeacherAvailability;

public record TeacherAvailabilityConfig(SchoolDayOfWeek DayOfWeek, int SlotNumber);

public record SetTeacherAvailabilityCommand(
    Guid TeacherId,
    List<TeacherAvailabilityConfig> Availabilities
) : IRequest<bool>;

public class SetTeacherAvailabilityCommandHandler : IRequestHandler<SetTeacherAvailabilityCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public SetTeacherAvailabilityCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(SetTeacherAvailabilityCommand request, CancellationToken cancellationToken)
    {
        // Clear existing availability for this teacher
        var existing = _context.TeacherAvailabilities.Where(a => a.TeacherId == request.TeacherId).ToList();
        foreach (var item in existing)
        {
            _context.TeacherAvailabilities.Remove(item);
        }

        // Add new availability
        foreach (var config in request.Availabilities)
        {
            var availability = new TeacherAvailability(Guid.NewGuid(), request.TeacherId, config.DayOfWeek, config.SlotNumber);
            _context.TeacherAvailabilities.Add(availability);
        }

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
