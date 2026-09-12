using MediatR;
using SchoolScheduler.Application.Common;
using SchoolScheduler.Application.Teachers.Dtos;
using System;

namespace SchoolScheduler.Application.Teachers.Queries.GetTeachersConfigs;

public record GetTeachersConfigsQuery(
    Guid SchoolId,
    int Page = 1,
    int PageSize = 50) : IRequest<PaginatedList<TeacherConfigDto>>;
