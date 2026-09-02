using SchoolScheduler.Application.Common;
using SchoolScheduler.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolScheduler.Application.Common.Interfaces;

public interface ITimetableGenerationService
{
    Task<TimetableGenerationResult> GenerateAsync(Guid schoolId, CancellationToken cancellationToken);
    Task<TimetableGenerationResult> GenerateForClassRoomAsync(Guid classRoomId, CancellationToken cancellationToken);
}
