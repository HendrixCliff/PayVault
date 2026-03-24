using System;
using System.Collections.Generic;

namespace PayVault.Domain.Exceptions
{
    public class ValidationException : Exception
    {
        public IEnumerable<string> Errors { get; }

        public ValidationException(string message) : base(message)
        {
            Errors = new List<string> { message };
        }

        public ValidationException(IEnumerable<string> errors) : base("One or more validation failures have occurred.")
        {
            Errors = errors;
        }
    }
}