using MediatR;
using SchoolScheduler.Application.Common.Interfaces;
using SchoolScheduler.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolScheduler.Application.Schools.Queries.GetWorkWeekConfig;

public record WorkWeekConfigDto(List<SchoolDayOfWeek> WorkingDays);

public record GetWorkWeekConfigQuery(Guid SchoolId) : IRequest<WorkWeekConfigDto>;

public class GetWorkWeekConfigQueryHandler : IRequestHandler<GetWorkWeekConfigQuery, WorkWeekConfigDto>
{
    private readonly IApplicationDbContext _context;

    public GetWorkWeekConfigQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<WorkWeekConfigDto> Handle(GetWorkWeekConfigQuery request, CancellationToken cancellationToken)
    {
        var workingDays = await _context.SchoolWorkingDays
            .Where(d => d.SchoolId == request.SchoolId)
            .Select(d => d.DayOfWeek)
            .ToListAsync(cancellationToken);

        return new WorkWeekConfigDto(workingDays);
    }
}
