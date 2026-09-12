using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SchoolScheduler.Application.Assignments.Commands.CreateClassSubjectAssignment;

namespace SchoolScheduler.Application.Assignments.Commands.BulkCreateClassSubjectAssignment;

public record BulkCreateClassSubjectAssignmentCommand(
    List<CreateClassSubjectAssignmentCommand> Assignments
) : IRequest<int>;
