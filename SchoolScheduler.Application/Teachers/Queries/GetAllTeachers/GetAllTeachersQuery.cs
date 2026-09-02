using MediatR;
using SchoolScheduler.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SchoolScheduler.Application.Teachers;

namespace SchoolScheduler.Application.Teachers.Queries.GetAllTeachers;

public record GetAllTeachersQuery(Guid SchoolId) : IRequest<List<TeacherDto>>;

public class GetAllTeachersQueryHandler : IRequestHandler<GetAllTeachersQuery, List<TeacherDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAllTeachersQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<TeacherDto>> Handle(GetAllTeachersQuery request, CancellationToken cancellationToken)
    {
        return await _context.Teachers
            .Where(t => t.SchoolId == request.SchoolId)
            .OrderBy(t => t.DisplayName)
            .Select(t => new TeacherDto(t.Id, t.SchoolId, t.DisplayName))
            .ToListAsync(cancellationToken);
    }
}
