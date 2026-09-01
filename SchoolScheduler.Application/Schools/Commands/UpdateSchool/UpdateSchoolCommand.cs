using MediatR;
using SchoolScheduler.Application.Common.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SchoolScheduler.Application.Schools.Commands.UpdateSchool;

public record UpdateSchoolCommand(Guid SchoolId, string Name, int LecturesPerDay) : IRequest<bool>;

public class UpdateSchoolCommandHandler : IRequestHandler<UpdateSchoolCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public UpdateSchoolCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(UpdateSchoolCommand request, CancellationToken cancellationToken)
    {
        var school = await _context.Schools
            .FirstOrDefaultAsync(s => s.Id == request.SchoolId, cancellationToken);

        if (school == null)
        {
            return false; // Not found
        }

        school.Update(request.Name, request.LecturesPerDay);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
