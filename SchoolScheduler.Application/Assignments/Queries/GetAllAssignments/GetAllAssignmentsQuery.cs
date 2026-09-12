using MediatR;
using SchoolScheduler.Application.Assignments;
using System;
using System.Collections.Generic;

namespace SchoolScheduler.Application.Assignments.Queries.GetAllAssignments;

public record GetAllAssignmentsQuery(Guid SchoolId) : IRequest<List<ClassSubjectAssignmentDto>>;
