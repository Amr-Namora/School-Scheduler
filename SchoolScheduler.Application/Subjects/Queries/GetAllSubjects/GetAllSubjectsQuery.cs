using MediatR;
using SchoolScheduler.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SchoolScheduler.Application.Subjects;

namespace SchoolScheduler.Application.Subjects.Queries.GetAllSubjects;

public record GetAllSubjectsQuery(Guid SchoolId) : IRequest<List<SubjectDto>>;

public class GetAllSubjectsQueryHandler : IRequestHandler<GetAllSubjectsQuery, List<SubjectDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAllSubjectsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<SubjectDto>> Handle(GetAllSubjectsQuery request, CancellationToken cancellationToken)
    {
        return await _context.Subjects
            .Where(s => s.SchoolId == request.SchoolId)
            .OrderBy(s => s.Name)
            .Select(s => new SubjectDto(s.Id, s.SchoolId, s.Name))
            .ToListAsync(cancellationToken);
    }
}
