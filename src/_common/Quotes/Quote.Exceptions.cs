namespace Skender.Stock.Indicators;

/// <summary>
/// Exception thrown when invalid quotes are encountered.
/// </summary>
[Serializable]
public class InvalidQuotesException : ArgumentOutOfRangeException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidQuotesException"/> class.
    /// </summary>
    public InvalidQuotesException() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidQuotesException"/> class with the name of the parameter that causes this exception.
    /// </summary>
    /// <param name="paramName">The name of the parameter that causes this exception.</param>
    public InvalidQuotesException(string? paramName)
        : base(paramName) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidQuotesException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for this exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public InvalidQuotesException(string? message, Exception? innerException)
        : base(message, innerException) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidQuotesException"/> class with the name of the parameter that causes this exception and a specified error message.
    /// </summary>
    /// <param name="paramName">The name of the parameter that causes this exception.</param>
    /// <param name="message">The error message that explains the reason for this exception.</param>
    public InvalidQuotesException(string? paramName, string? message)
        : base(paramName, message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidQuotesException"/> class with the name of the parameter that causes this exception, the value of the argument, and a specified error message.
    /// </summary>
    /// <param name="paramName">The name of the parameter that causes this exception.</param>
    /// <param name="actualValue">The value of the argument that causes this exception.</param>
    /// <param name="message">The error message that explains the reason for this exception.</param>
    public InvalidQuotesException(string? paramName, object? actualValue, string? message)
        : base(paramName, actualValue, message) { }
}
