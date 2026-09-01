using MediatR;
using SchoolScheduler.Application.Common.Interfaces;
using SchoolScheduler.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolScheduler.Application.Schools.Commands.CreateSchool;

public record CreateSchoolCommand(string Name, int LecturesPerDay) : IRequest<Guid>;

public class CreateSchoolCommandHandler : IRequestHandler<CreateSchoolCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateSchoolCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateSchoolCommand request, CancellationToken cancellationToken)
    {
        var school = new School(Guid.NewGuid(), request.Name, request.LecturesPerDay);
        _context.Schools.Add(school);
        await _context.SaveChangesAsync(cancellationToken);
        return school.Id;
    }
}
