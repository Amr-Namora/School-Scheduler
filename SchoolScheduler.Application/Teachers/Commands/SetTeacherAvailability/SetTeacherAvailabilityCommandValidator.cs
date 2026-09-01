using FluentValidation;
using SchoolScheduler.Application.Teachers.Commands.SetTeacherAvailability;

namespace SchoolScheduler.Application.Teachers.Commands.SetTeacherAvailability;

public class SetTeacherAvailabilityCommandValidator : AbstractValidator<SetTeacherAvailabilityCommand>
{
    public SetTeacherAvailabilityCommandValidator()
    {
        RuleFor(x => x.TeacherId).NotEmpty();
        RuleFor(x => x.Availabilities).NotEmpty().WithMessage("At least one availability slot must be provided.");

        RuleForEach(x => x.Availabilities).ChildRules(avail =>
        {
            avail.RuleFor(a => a.SlotNumber).GreaterThan(0);
        });
    }
}
