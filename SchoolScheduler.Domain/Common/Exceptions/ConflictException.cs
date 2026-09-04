using System;

namespace SchoolScheduler.Domain.Common.Exceptions;

public class ConflictException : Exception
{
    public ConflictException(string message) : base(message) { }
}
