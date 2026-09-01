using MediatR;
using SchoolScheduler.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
using SchoolScheduler.Application.Subjects;

namespace SchoolScheduler.Application.Subjects.Queries.GetSubjectById;

public record GetSubjectByIdQuery(Guid Id) : IRequest<SubjectDto>;

public class GetSubjectByIdQueryHandler : IRequestHandler<GetSubjectByIdQuery, SubjectDto>
{
    private readonly IApplicationDbContext _context;

    public GetSubjectByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SubjectDto> Handle(GetSubjectByIdQuery request, CancellationToken cancellationToken)
    {
        var subject = await _context.Subjects
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

        if (subject == null)
        {
            return null;
        }

        return new SubjectDto(subject.Id, subject.SchoolId, subject.Name);
    }
}
