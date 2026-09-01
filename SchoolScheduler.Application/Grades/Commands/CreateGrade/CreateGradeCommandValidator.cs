using FluentValidation;
using SchoolScheduler.Application.Grades.Commands.CreateGrade;

namespace SchoolScheduler.Application.Grades.Commands.CreateGrade;

public class CreateGradeCommandValidator : AbstractValidator<CreateGradeCommand>
{
    public CreateGradeCommandValidator()
    {
        RuleFor(x => x.SchoolId).NotEmpty();
        RuleFor(x => x.Category).NotNull();
        RuleFor(x => x.Level).GreaterThan(0).WithMessage("Grade level must be positive.");
    }
}
