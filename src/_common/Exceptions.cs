using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Skender.Stock.Indicators;

public class InvalidQuotesException : ArgumentOutOfRangeException
{
    public InvalidQuotesException()
    {
    }

    public InvalidQuotesException(string? paramName)
        : base(paramName)
    {
    }

    public InvalidQuotesException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }

    public InvalidQuotesException(string? paramName, string? message)
        : base(paramName, message)
    {
    }

    public InvalidQuotesException(string? paramName, object? actualValue, string? message)
        : base(paramName, actualValue, message)
    {
    }

    // A constructor is needed for serialization when an
    // exception propagates from a remoting server to the client.
    [ExcludeFromCodeCoverage]
    protected InvalidQuotesException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
