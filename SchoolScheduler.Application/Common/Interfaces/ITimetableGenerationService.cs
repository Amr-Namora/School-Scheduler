using SchoolScheduler.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolScheduler.Application.Common.Interfaces;

public interface ITimetableGenerationService
{
    Task<IEnumerable<TimetableEntry>> GenerateAsync(Guid schoolId, CancellationToken cancellationToken);
    Task<IEnumerable<TimetableEntry>> GenerateForClassRoomAsync(Guid classRoomId, CancellationToken cancellationToken);
}
