using FluentValidation;
using SchoolScheduler.Application.Subjects.Commands.UpdateSubject;
using SchoolScheduler.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolScheduler.Application.Subjects.Commands.UpdateSubject;

public class UpdateSubjectCommandValidator : AbstractValidator<UpdateSubjectCommand>
{
    public UpdateSubjectCommandValidator(IApplicationDbContext context)
    {
        RuleFor(x => x.SubjectId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().WithMessage("Subject name is required.");

        RuleFor(x => x).MustAsync(async (cmd, cancellation) =>
        {
            var exists = await context.Subjects.AnyAsync(s => s.Id == cmd.SubjectId, cancellation);
            return exists;
        }).WithMessage("Subject not found.");
    }
}
