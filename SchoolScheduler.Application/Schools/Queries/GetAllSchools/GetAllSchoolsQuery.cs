using MediatR;
using SchoolScheduler.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SchoolScheduler.Application.Schools;

namespace SchoolScheduler.Application.Schools.Queries.GetAllSchools;

public record GetAllSchoolsQuery() : IRequest<List<SchoolDto>>;

public class GetAllSchoolsQueryHandler : IRequestHandler<GetAllSchoolsQuery, List<SchoolDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAllSchoolsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<SchoolDto>> Handle(GetAllSchoolsQuery request, CancellationToken cancellationToken)
    {
        return await _context.Schools
            .Select(s => new SchoolDto(s.Id, s.Name, s.LecturesPerDay))
            .ToListAsync(cancellationToken);
    }
}
