using FluentValidation;
using SchoolScheduler.Application.ClassRooms.Commands.DeleteClassRoom;
using SchoolScheduler.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolScheduler.Application.ClassRooms.Commands.DeleteClassRoom;

public class DeleteClassRoomCommandValidator : AbstractValidator<DeleteClassRoomCommand>
{
    public DeleteClassRoomCommandValidator(IApplicationDbContext context)
    {
        RuleFor(x => x.ClassRoomId).NotEmpty();

        RuleFor(x => x).MustAsync(async (cmd, cancellation) =>
        {
            var exists = await context.ClassRooms.AnyAsync(c => c.Id == cmd.ClassRoomId, cancellation);
            return exists;
        }).WithMessage("Class room not found.");
    }
}
