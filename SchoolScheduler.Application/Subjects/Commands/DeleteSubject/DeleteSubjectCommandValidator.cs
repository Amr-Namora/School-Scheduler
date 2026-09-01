using FluentValidation;
using SchoolScheduler.Application.Subjects.Commands.DeleteSubject;
using SchoolScheduler.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolScheduler.Application.Subjects.Commands.DeleteSubject;

public class DeleteSubjectCommandValidator : AbstractValidator<DeleteSubjectCommand>
{
    public DeleteSubjectCommandValidator(IApplicationDbContext context)
    {
        RuleFor(x => x.SubjectId).NotEmpty();

        RuleFor(x => x).MustAsync(async (cmd, cancellation) =>
        {
            var exists = await context.Subjects.AnyAsync(s => s.Id == cmd.SubjectId, cancellation);
            return exists;
        }).WithMessage("Subject not found.");
    }
}
