using FluentValidation;
using SchoolScheduler.Application.Schools.Commands.ConfigureWorkWeek;
using System.Linq;

namespace SchoolScheduler.Application.Schools.Commands.ConfigureWorkWeek;

public class ConfigureWorkWeekCommandValidator : AbstractValidator<ConfigureWorkWeekCommand>
{
    public ConfigureWorkWeekCommandValidator()
    {
        RuleFor(x => x.SchoolId).NotEmpty();
        RuleFor(x => x.DayConfigs).NotEmpty().WithMessage("At least one working day must be configured.");

        RuleForEach(x => x.DayConfigs).ChildRules(day =>
        {
            day.RuleFor(d => d.LectureSlots).NotEmpty().WithMessage("Each working day must have at least one lecture slot.");
            day.RuleForEach(d => d.LectureSlots).ChildRules(slot =>
            {
                slot.RuleFor(s => s.SlotNumber).GreaterThan(0);
                slot.RuleFor(s => s.StartTime).LessThan(s => s.EndTime).WithMessage("Start time must be before end time.");
            });
            day.RuleForEach(d => d.BreakSlots).ChildRules(brk =>
            {
                brk.RuleFor(b => b.AfterSlotNumber).GreaterThan(0);
                brk.RuleFor(b => b.StartTime).LessThan(b => b.EndTime).WithMessage("Start time must be before end time.");
            });
        });
    }
}
