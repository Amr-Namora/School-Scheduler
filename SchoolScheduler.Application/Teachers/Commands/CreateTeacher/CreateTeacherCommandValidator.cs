using FluentValidation;
using SchoolScheduler.Application.Teachers.Commands.CreateTeacher;

namespace SchoolScheduler.Application.Teachers.Commands.CreateTeacher;

public class CreateTeacherCommandValidator : AbstractValidator<CreateTeacherCommand>
{
    public CreateTeacherCommandValidator()
    {
        RuleFor(x => x.SchoolId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().WithMessage("Teacher name is required.");
    }
}
