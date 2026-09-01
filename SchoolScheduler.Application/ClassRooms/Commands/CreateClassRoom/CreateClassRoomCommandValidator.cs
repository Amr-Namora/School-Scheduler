using FluentValidation;
using SchoolScheduler.Application.ClassRooms.Commands.CreateClassRoom;

namespace SchoolScheduler.Application.ClassRooms.Commands.CreateClassRoom;

public class CreateClassRoomCommandValidator : AbstractValidator<CreateClassRoomCommand>
{
    public CreateClassRoomCommandValidator()
    {
        RuleFor(x => x.SchoolId).NotEmpty();
        RuleFor(x => x.GradeId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().WithMessage("Class room name is required.");
    }
}
