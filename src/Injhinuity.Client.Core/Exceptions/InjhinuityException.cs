using System;

namespace Injhinuity.Client.Core.Exceptions
{
    public class InjhinuityException : Exception
    {
        public InjhinuityException() : base() {}

        public InjhinuityException(string message) : base(message) {}

        public InjhinuityException(string message, Exception innerException) : base(message, innerException) {}
    }
}
