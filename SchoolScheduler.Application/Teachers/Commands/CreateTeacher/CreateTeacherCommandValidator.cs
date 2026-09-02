using FluentValidation;
using SchoolScheduler.Application.Teachers.Commands.CreateTeacher;

namespace SchoolScheduler.Application.Teachers.Commands.CreateTeacher;

public class CreateTeacherCommandValidator : AbstractValidator<CreateTeacherCommand>
{
    public CreateTeacherCommandValidator()
    {
        RuleFor(x => x.SchoolId).NotEmpty();
        RuleFor(x => x.DisplayName).NotEmpty().WithMessage("Teacher display name is required.");
        RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("A valid teacher email is required.");
    }
}
