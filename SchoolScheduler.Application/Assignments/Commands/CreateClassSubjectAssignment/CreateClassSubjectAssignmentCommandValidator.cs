using FluentValidation;
using SchoolScheduler.Application.Assignments.Commands.CreateClassSubjectAssignment;
using SchoolScheduler.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolScheduler.Application.Assignments.Commands.CreateClassSubjectAssignment;

public class CreateClassSubjectAssignmentCommandValidator : AbstractValidator<CreateClassSubjectAssignmentCommand>
{
    public CreateClassSubjectAssignmentCommandValidator(IApplicationDbContext context)
    {
        RuleFor(x => x.ClassRoomId).NotEmpty();
        RuleFor(x => x.SubjectId).NotEmpty();
        RuleFor(x => x.TeacherId).NotEmpty();
        RuleFor(x => x.WeeklyQuota).GreaterThan(0);

        // Rule: One teacher per subject per classroom
        RuleFor(x => x).MustAsync(async (cmd, cancellation) =>
        {
            var exists = await context.ClassSubjectAssignments
                .AnyAsync(a => a.ClassRoomId == cmd.ClassRoomId && a.SubjectId == cmd.SubjectId, cancellation);
            return !exists;
        }).WithMessage("This subject is already assigned to this classroom.");

        // Rule: Weekly quota cannot exceed teacher's total available slots
        RuleFor(x => x).MustAsync(async (cmd, cancellation) =>
        {
            var availableSlotsCount = await context.TeacherAvailabilities
                .CountAsync(a => a.TeacherId == cmd.TeacherId, cancellation);

            var currentTotalQuota = await context.ClassSubjectAssignments
                .Where(a => a.TeacherId == cmd.TeacherId)
                .SumAsync(a => a.WeeklyQuota, cancellation);

            return (currentTotalQuota + cmd.WeeklyQuota) <= availableSlotsCount;
        }).WithMessage("The requested quota exceeds the teacher's total weekly availability.");
    }
}
