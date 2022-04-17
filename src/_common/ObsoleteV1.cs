using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Skender.Stock.Indicators;

// RENAMED IN v1.23.0 - .ConvertToQuotes()
[ExcludeFromCodeCoverage]
public static partial class Indicator
{
    [Obsolete("Rename 'ConvertToQuotes()' to 'ToQuotes()' to fix.")]
    public static IEnumerable<Quote> ConvertToQuotes(
        this IEnumerable<AdlResult> results)
    {
        return results.ToQuotes();
    }

    [Obsolete("Rename 'ConvertToQuotes()' to 'ToQuotes()' to fix.")]
    public static IEnumerable<Quote> ConvertToQuotes(
        this IEnumerable<DpoResult> results)
    {
        return results.ToQuotes();
    }

    [Obsolete("'ConvertToQuotes()' is not needed for Heikin-Ashi.  Results can now be used directly as a replacement for IEnumerable<TQuote>.")]
    public static IEnumerable<Quote> ConvertToQuotes(
        this IEnumerable<HeikinAshiResult> results)
    {
        return results
          .Select(x => new Quote
          {
              Date = x.Date,
              Open = x.Open,
              High = x.High,
              Low = x.Low,
              Close = x.Close,
              Volume = x.Volume
          })
          .ToList();
    }

    [Obsolete("Rename 'ConvertToQuotes()' to 'ToQuotes()' to fix.")]
    public static IEnumerable<Quote> ConvertToQuotes(
        this IEnumerable<HurstResult> results)
    {
        return results.ToQuotes();
    }

    [Obsolete("Rename 'ConvertToQuotes()' to 'ToQuotes()' to fix.")]
    public static IEnumerable<Quote> ConvertToQuotes(
        this IEnumerable<ObvResult> results)
    {
        return results.ToQuotes();
    }

    [Obsolete("'ConvertToQuotes()' is not needed for Renko.  Results can now be used directly as a replacement for IEnumerable<TQuote>.")]
    public static IEnumerable<Quote> ConvertToQuotes(
        this IEnumerable<RenkoResult> results)
    {
        return results
          .Select(x => new Quote
          {
              Date = x.Date,
              Open = x.Open,
              High = x.High,
              Low = x.Low,
              Close = x.Close,
              Volume = x.Volume
          })
          .ToList();
    }

    [Obsolete("Rename 'ConvertToQuotes()' to 'ToQuotes()' to fix.")]
    public static IEnumerable<Quote> ConvertToQuotes(
        this IEnumerable<RsiResult> results)
    {
        return results.ToQuotes();
    }
}

// REMOVED in v1.21.0
[Obsolete("Using less than recommended quote history no longer throws an exception.  "
        + "See https://github.com/DaveSkender/Stock.Indicators/pull/685 for more info.")]
[ExcludeFromCodeCoverage]
public class BadQuotesException : ArgumentOutOfRangeException
{
    public BadQuotesException()
    {
    }

    public BadQuotesException(string? paramName)
        : base(paramName)
    {
    }

    public BadQuotesException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }

    public BadQuotesException(string? paramName, string? message)
        : base(paramName, message)
    {
    }

    public BadQuotesException(string? paramName, object? actualValue, string? message)
        : base(paramName, actualValue, message)
    {
    }

    // A constructor is needed for serialization when an
    // exception propagates from a remoting server to the client.
    [ExcludeFromCodeCoverage] // TODO: how do you test this?
    protected BadQuotesException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}