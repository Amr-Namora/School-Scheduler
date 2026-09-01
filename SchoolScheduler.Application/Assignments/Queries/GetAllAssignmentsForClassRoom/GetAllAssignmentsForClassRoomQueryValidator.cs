using FluentValidation;
using SchoolScheduler.Application.Assignments.Queries.GetAllAssignmentsForClassRoom;
using SchoolScheduler.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolScheduler.Application.Assignments.Queries.GetAllAssignmentsForClassRoom;

public class GetAllAssignmentsForClassRoomQueryValidator : AbstractValidator<GetAllAssignmentsForClassRoomQuery>
{
    public GetAllAssignmentsForClassRoomQueryValidator(IApplicationDbContext context)
    {
        RuleFor(x => x.ClassRoomId).NotEmpty();

        RuleFor(x => x).MustAsync(async (cmd, cancellation) =>
        {
            var exists = await context.ClassRooms.AnyAsync(c => c.Id == cmd.ClassRoomId, cancellation);
            return exists;
        }).WithMessage("Classroom not found.");
    }
}
