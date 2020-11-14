using System;
using System.Runtime.Serialization;

#nullable enable

namespace Skender.Stock.Indicators
{
    // ref: https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/exceptions/creating-and-throwing-exceptions#defining-exception-classes

    [Serializable()]
    public class BadHistoryException : ArgumentOutOfRangeException
    {
        public BadHistoryException() { }

        public BadHistoryException(string? paramName)
            : base(paramName) { }

        public BadHistoryException(string? message, Exception? innerException)
            : base(message, innerException) { }

        public BadHistoryException(string? paramName, string? message)
            : base(paramName, message) { }

        public BadHistoryException(string? paramName, object? actualValue, string? message)
            : base(paramName, actualValue, message) { }

        // A constructor is needed for serialization when an
        // exception propagates from a remoting server to the client. 
        protected BadHistoryException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
