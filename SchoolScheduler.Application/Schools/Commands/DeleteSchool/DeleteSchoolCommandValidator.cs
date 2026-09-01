using FluentValidation;
using SchoolScheduler.Application.Schools.Commands.DeleteSchool;
using SchoolScheduler.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolScheduler.Application.Schools.Commands.DeleteSchool;

public class DeleteSchoolCommandValidator : AbstractValidator<DeleteSchoolCommand>
{
    public DeleteSchoolCommandValidator(IApplicationDbContext context)
    {
        RuleFor(x => x.SchoolId).NotEmpty();

        RuleFor(x => x).MustAsync(async (cmd, cancellation) =>
        {
            var exists = await context.Schools.AnyAsync(s => s.Id == cmd.SchoolId, cancellation);
            return exists;
        }).WithMessage("School not found.");
    }
}
