using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SchoolScheduler.Application.Common.Exceptions;

public class ValidationException : Exception
{
    public List<ValidationFailure> Errors { get; }

    public ValidationException(List<ValidationFailure> errors)
        : base("One or more validation failures have occurred.")
    {
        Errors = errors;
    }
}
