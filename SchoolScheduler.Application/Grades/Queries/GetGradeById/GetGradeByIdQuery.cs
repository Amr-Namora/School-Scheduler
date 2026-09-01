using MediatR;
using SchoolScheduler.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
using SchoolScheduler.Application.Grades;

namespace SchoolScheduler.Application.Grades.Queries.GetGradeById;

public record GetGradeByIdQuery(Guid Id) : IRequest<GradeDto>;

public class GetGradeByIdQueryHandler : IRequestHandler<GetGradeByIdQuery, GradeDto>
{
    private readonly IApplicationDbContext _context;

    public GetGradeByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<GradeDto> Handle(GetGradeByIdQuery request, CancellationToken cancellationToken)
    {
        var grade = await _context.Grades
            .FirstOrDefaultAsync(g => g.Id == request.Id, cancellationToken);

        if (grade == null)
        {
            return null;
        }

        return new GradeDto(grade.Id, grade.SchoolId, grade.Category, grade.Level);
    }
}
