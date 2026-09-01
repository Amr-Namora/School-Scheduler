using FluentValidation;
using SchoolScheduler.Application.Teachers.Commands.AssignSubjectsToTeacher;

namespace SchoolScheduler.Application.Teachers.Commands.AssignSubjectsToTeacher;

public class AssignSubjectsToTeacherCommandValidator : AbstractValidator<AssignSubjectsToTeacherCommand>
{
    public AssignSubjectsToTeacherCommandValidator()
    {
        RuleFor(x => x.TeacherId).NotEmpty();
        RuleFor(x => x.SubjectIds).NotEmpty().WithMessage("At least one subject must be assigned.");
    }
}
