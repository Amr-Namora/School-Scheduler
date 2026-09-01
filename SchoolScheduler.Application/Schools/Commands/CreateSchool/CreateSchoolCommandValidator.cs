using FluentValidation;
using SchoolScheduler.Application.Schools.Commands.CreateSchool;

namespace SchoolScheduler.Application.Schools.Commands.CreateSchool;

public class CreateSchoolCommandValidator : AbstractValidator<CreateSchoolCommand>
{
    public CreateSchoolCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("School name is required.");
        RuleFor(x => x.LecturesPerDay).GreaterThan(0).WithMessage("Lectures per day must be at least 1.");
    }
}
