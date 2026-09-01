using FluentValidation;
using SchoolScheduler.Application.Teachers.Queries.GetAvailabilityForTeacher;
using SchoolScheduler.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolScheduler.Application.Teachers.Queries.GetAvailabilityForTeacher;

public class GetAvailabilityForTeacherQueryValidator : AbstractValidator<GetAvailabilityForTeacherQuery>
{
    public GetAvailabilityForTeacherQueryValidator(IApplicationDbContext context)
    {
        RuleFor(x => x.TeacherId).NotEmpty();

        RuleFor(x => x).MustAsync(async (cmd, cancellation) =>
        {
            var exists = await context.Teachers.AnyAsync(t => t.Id == cmd.TeacherId, cancellation);
            return exists;
        }).WithMessage("Teacher not found.");
    }
}
