using FluentValidation;
using SchoolScheduler.Application.Assignments.Commands.DeleteClassSubjectAssignment;
using SchoolScheduler.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolScheduler.Application.Assignments.Commands.DeleteClassSubjectAssignment;

public class DeleteClassSubjectAssignmentCommandValidator : AbstractValidator<DeleteClassSubjectAssignmentCommand>
{
    public DeleteClassSubjectAssignmentCommandValidator(IApplicationDbContext context)
    {
        RuleFor(x => x.Id).NotEmpty();

        RuleFor(x => x).MustAsync(async (cmd, cancellation) =>
        {
            var exists = await context.ClassSubjectAssignments.AnyAsync(a => a.Id == cmd.Id, cancellation);
            return exists;
        }).WithMessage("Assignment not found.");
    }
}
