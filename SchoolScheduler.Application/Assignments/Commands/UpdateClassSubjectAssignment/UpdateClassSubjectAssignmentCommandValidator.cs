using FluentValidation;
using SchoolScheduler.Application.Assignments.Commands.UpdateClassSubjectAssignment;
using SchoolScheduler.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolScheduler.Application.Assignments.Commands.UpdateClassSubjectAssignment;

public class UpdateClassSubjectAssignmentCommandValidator : AbstractValidator<UpdateClassSubjectAssignmentCommand>
{
    public UpdateClassSubjectAssignmentCommandValidator(IApplicationDbContext context)
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.TeacherId).NotEmpty();
        RuleFor(x => x.SubjectId).NotEmpty();
        RuleFor(x => x.WeeklyQuota).GreaterThan(0);

        RuleFor(x => x).MustAsync(async (cmd, cancellation) =>
        {
            var exists = await context.ClassSubjectAssignments.AnyAsync(a => a.Id == cmd.Id, cancellation);
            return exists;
        }).WithMessage("Assignment not found.");

        RuleFor(x => x).MustAsync(async (cmd, cancellation) =>
        {
            var teacherExists = await context.Teachers.AnyAsync(t => t.Id == cmd.TeacherId, cancellation);
            if (!teacherExists) return false;

            var subjectExists = await context.Subjects.AnyAsync(s => s.Id == cmd.SubjectId, cancellation);
            if (!subjectExists) return false;

            return true;
        }).WithMessage("Teacher or Subject not found.");

        // Capacity Check: (sum of other assignments for the target teacher + new quota) <= teacher's availability
        RuleFor(x => x).MustAsync(async (cmd, cancellation) =>
        {
            var availableSlotsCount = await context.TeacherAvailabilities
                .CountAsync(a => a.TeacherId == cmd.TeacherId, cancellation);

            var currentTotalQuota = await context.ClassSubjectAssignments
                .Where(a => a.TeacherId == cmd.TeacherId && a.Id != cmd.Id)
                .SumAsync(a => a.WeeklyQuota, cancellation);

            return (currentTotalQuota + cmd.WeeklyQuota) <= availableSlotsCount;
        }).WithMessage("The updated quota exceeds the teacher's total weekly availability.");
    }
}
