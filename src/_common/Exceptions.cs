using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

#nullable enable

namespace Skender.Stock.Indicators
{

    [Serializable]
    public class BadQuotesException : ArgumentOutOfRangeException
    {
        public BadQuotesException() { }

        public BadQuotesException(string? paramName)
            : base(paramName) { }

        public BadQuotesException(string? message, Exception? innerException)
            : base(message, innerException) { }

        public BadQuotesException(string? paramName, string? message)
            : base(paramName, message) { }

        public BadQuotesException(string? paramName, object? actualValue, string? message)
            : base(paramName, actualValue, message) { }

        // A constructor is needed for serialization when an
        // exception propagates from a remoting server to the client.
        [ExcludeFromCodeCoverage]  // TODO: how do you test this?
        protected BadQuotesException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
