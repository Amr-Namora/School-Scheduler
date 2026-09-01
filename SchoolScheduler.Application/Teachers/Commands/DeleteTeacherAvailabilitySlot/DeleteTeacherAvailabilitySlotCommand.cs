using MediatR;
using SchoolScheduler.Application.Common.Interfaces;
using SchoolScheduler.Domain.Entities;
using SchoolScheduler.Domain.Enums;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SchoolScheduler.Application.Teachers.Commands.DeleteTeacherAvailabilitySlot;

public record DeleteTeacherAvailabilitySlotCommand(
    Guid TeacherId,
    SchoolDayOfWeek DayOfWeek,
    int SlotNumber
) : IRequest<bool>;

public class DeleteTeacherAvailabilitySlotCommandHandler : IRequestHandler<DeleteTeacherAvailabilitySlotCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public DeleteTeacherAvailabilitySlotCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteTeacherAvailabilitySlotCommand request, CancellationToken cancellationToken)
    {
        var availability = await _context.TeacherAvailabilities
            .FirstOrDefaultAsync(a => a.TeacherId == request.TeacherId
                                   && a.DayOfWeek == request.DayOfWeek
                                   && a.SlotNumber == request.SlotNumber, cancellationToken);

        if (availability == null)
        {
            return false;
        }

        _context.TeacherAvailabilities.Remove(availability);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
