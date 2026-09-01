using MediatR;
using SchoolScheduler.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SchoolScheduler.Application.Grades;

namespace SchoolScheduler.Application.Grades.Queries.GetAllGrades;

public record GetAllGradesQuery(Guid SchoolId) : IRequest<List<GradeDto>>;

public class GetAllGradesQueryHandler : IRequestHandler<GetAllGradesQuery, List<GradeDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAllGradesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<GradeDto>> Handle(GetAllGradesQuery request, CancellationToken cancellationToken)
    {
        return await _context.Grades
            .Where(g => g.SchoolId == request.SchoolId)
            .OrderBy(g => g.Level)
            .Select(g => new GradeDto(g.Id, g.SchoolId, g.Category, g.Level))
            .ToListAsync(cancellationToken);
    }
}
