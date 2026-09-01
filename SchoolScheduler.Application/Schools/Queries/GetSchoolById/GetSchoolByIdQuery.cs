using MediatR;
using SchoolScheduler.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
using SchoolScheduler.Application.Schools;

namespace SchoolScheduler.Application.Schools.Queries.GetSchoolById;

public record GetSchoolByIdQuery(Guid Id) : IRequest<SchoolDto>;

public class GetSchoolByIdQueryHandler : IRequestHandler<GetSchoolByIdQuery, SchoolDto>
{
    private readonly IApplicationDbContext _context;

    public GetSchoolByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SchoolDto> Handle(GetSchoolByIdQuery request, CancellationToken cancellationToken)
    {
        var school = await _context.Schools
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

        if (school == null)
        {
            return null; // The API layer can translate this to 404
        }

        return new SchoolDto(school.Id, school.Name, school.LecturesPerDay);
    }
}
