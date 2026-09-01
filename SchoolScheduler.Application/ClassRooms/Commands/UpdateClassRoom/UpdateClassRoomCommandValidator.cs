using FluentValidation;
using SchoolScheduler.Application.ClassRooms.Commands.UpdateClassRoom;
using SchoolScheduler.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolScheduler.Application.ClassRooms.Commands.UpdateClassRoom;

public class UpdateClassRoomCommandValidator : AbstractValidator<UpdateClassRoomCommand>
{
    public UpdateClassRoomCommandValidator(IApplicationDbContext context)
    {
        RuleFor(x => x.ClassRoomId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().WithMessage("Class room name is required.");

        RuleFor(x => x).MustAsync(async (cmd, cancellation) =>
        {
            var exists = await context.ClassRooms.AnyAsync(c => c.Id == cmd.ClassRoomId, cancellation);
            return exists;
        }).WithMessage("Class room not found.");
    }
}
