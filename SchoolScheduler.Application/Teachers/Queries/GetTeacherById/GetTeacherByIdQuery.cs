using MediatR;
using SchoolScheduler.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
using SchoolScheduler.Application.Teachers;

namespace SchoolScheduler.Application.Teachers.Queries.GetTeacherById;

public record GetTeacherByIdQuery(Guid Id) : IRequest<TeacherDto>;

public class GetTeacherByIdQueryHandler : IRequestHandler<GetTeacherByIdQuery, TeacherDto>
{
    private readonly IApplicationDbContext _context;

    public GetTeacherByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<TeacherDto> Handle(GetTeacherByIdQuery request, CancellationToken cancellationToken)
    {
        var teacher = await _context.Teachers
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        if (teacher == null)
        {
            return null;
        }

        return new TeacherDto(teacher.Id, teacher.SchoolId, teacher.DisplayName, teacher.Email);
    }
}
