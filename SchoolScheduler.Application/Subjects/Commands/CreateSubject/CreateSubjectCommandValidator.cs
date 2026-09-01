using FluentValidation;
using SchoolScheduler.Application.Subjects.Commands.CreateSubject;

namespace SchoolScheduler.Application.Subjects.Commands.CreateSubject;

public class CreateSubjectCommandValidator : AbstractValidator<CreateSubjectCommand>
{
    public CreateSubjectCommandValidator()
    {
        RuleFor(x => x.SchoolId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().WithMessage("Subject name is required.");
    }
}
