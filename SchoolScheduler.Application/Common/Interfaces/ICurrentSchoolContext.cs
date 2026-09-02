using System;
using System.Threading.Tasks;

namespace SchoolScheduler.Application.Common.Interfaces;

public interface ICurrentSchoolContext
{
    Guid? SchoolId { get; }
    Task<Guid?> GetSchoolIdAsync();
}
