using FluentValidation;
using SchoolScheduler.Application.Timetable.Commands.GenerateTimetable;
using SchoolScheduler.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolScheduler.Application.Timetable.Commands.GenerateTimetable;

public class GenerateTimetableCommandValidator : AbstractValidator<GenerateTimetableCommand>
{
    public GenerateTimetableCommandValidator(IApplicationDbContext context)
    {
        RuleFor(x => x.SchoolId).NotEmpty();

        RuleFor(x => x).MustAsync(async (cmd, cancellation) =>
        {
            // Ensure at least one assignment exists before attempting to generate
            var assignmentsCount = await context.ClassSubjectAssignments
                .CountAsync(a => context.ClassRooms.Any(c => c.Id == a.ClassRoomId && c.SchoolId == cmd.SchoolId), cancellation);

            return assignmentsCount > 0;
        }).WithMessage("No class subject assignments found for this school. Please configure them first.");
    }
}
