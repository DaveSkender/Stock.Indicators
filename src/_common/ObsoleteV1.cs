using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
#nullable disable

namespace Skender.Stock.Indicators;

// REMOVED IN v1.24.0, ToQuotes(), CandlePart params
public static partial class Indicator
{
    [ExcludeFromCodeCoverage]
    [Obsolete("The use of CandlePart as a parameter is deprecated.  "
    + "Use '.Use(candlePart)' prior to calling .GetEma()")]
    public static IEnumerable<EmaResult> GetEma<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods,
        CandlePart candlePart)
        where TQuote : IQuote => quotes
            .ToBasicTuple(candlePart)
            .CalcEma(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("The use of CandlePart as a parameter is deprecated.  "
        + "Use '.Use(candlePart)' prior to calling .GetMacd()")]
    public static IEnumerable<MacdResult> GetMacd<TQuote>(
        this IEnumerable<TQuote> quotes,
        int fastPeriods,
        int slowPeriods,
        int signalPeriods,
        CandlePart candlePart)
        where TQuote : IQuote => quotes
            .ToBasicTuple(candlePart)
            .CalcMacd(fastPeriods, slowPeriods, signalPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("The use of CandlePart as a parameter is deprecated.  "
        + "Use '.Use(candlePart)' prior to calling .GetSma()")]
    public static IEnumerable<SmaResult> GetSma<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods,
        CandlePart candlePart)
        where TQuote : IQuote => quotes
            .ToBasicTuple(candlePart)
            .CalcSma(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("The use of CandlePart as a parameter is deprecated.  "
        + "Use '.Use(candlePart)' prior to calling .GetWma()")]
    public static IEnumerable<WmaResult> GetWma<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods,
        CandlePart candlePart)
        where TQuote : IQuote => quotes
            .ToBasicTuple(candlePart)
            .CalcWma(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("The .ToQuotes() utility is obsolete.  Use v2 chaining instead.")]
    public static IEnumerable<Quote> ToQuotes(
    this IEnumerable<AdlResult> results) => results
      .Select(x => new Quote
      {
          Date = x.Date,
          Open = (decimal)x.Adl,
          High = (decimal)x.Adl,
          Low = (decimal)x.Adl,
          Close = (decimal)x.Adl,
          Volume = (decimal)x.Adl
      })
      .ToList();

    [ExcludeFromCodeCoverage]
    [Obsolete("The .ToQuotes() utility is obsolete.  Use v2 chaining instead.")]
    public static IEnumerable<Quote> ToQuotes(
    this IEnumerable<DpoResult> results) => results
      .Where(x => x.Dpo != null)
      .Select(x => new Quote
      {
          Date = x.Date,
          Open = (decimal)x.Dpo,
          High = (decimal)x.Dpo,
          Low = (decimal)x.Dpo,
          Close = (decimal)x.Dpo,
          Volume = (decimal)x.Dpo
      })
      .ToList();

    [ExcludeFromCodeCoverage]
    [Obsolete("The .ToQuotes() utility is obsolete.  Use v2 chaining instead.")]
    public static IEnumerable<Quote> ToQuotes(
    this IEnumerable<HurstResult> results) => results
      .Where(x => x.HurstExponent != null)
      .Select(x => new Quote
      {
          Date = x.Date,
          Open = (decimal)x.HurstExponent,
          High = (decimal)x.HurstExponent,
          Low = (decimal)x.HurstExponent,
          Close = (decimal)x.HurstExponent
      })
      .ToList();

    [ExcludeFromCodeCoverage]
    [Obsolete("The .ToQuotes() utility is obsolete.  Use v2 chaining instead.")]
    public static IEnumerable<Quote> ToQuotes(
    this IEnumerable<ObvResult> results) => results
      .Select(x => new Quote
      {
          Date = x.Date,
          Open = (decimal)x.Obv,
          High = (decimal)x.Obv,
          Low = (decimal)x.Obv,
          Close = (decimal)x.Obv,
          Volume = (decimal)x.Obv
      })
      .ToList();

    [ExcludeFromCodeCoverage]
    [Obsolete("The .ToQuotes() utility is obsolete.  Use v2 chaining instead.")]
    public static IEnumerable<Quote> ToQuotes(
    this IEnumerable<RsiResult> results) => results
      .Where(x => x.Rsi != null)
      .Select(x => new Quote
      {
          Date = x.Date,
          Open = (decimal)x.Rsi,
          High = (decimal)x.Rsi,
          Low = (decimal)x.Rsi,
          Close = (decimal)x.Rsi,
          Volume = (decimal)x.Rsi
      })
      .ToList();
}

// RENAMED IN v1.23.0 - GetDoubleEma, GetTripleEma
public static partial class Indicator
{
    [ExcludeFromCodeCoverage]
    [Obsolete("Rename 'GetDoubleEma(..)' to 'GetDema(..)' to fix.")]
    public static IEnumerable<DemaResult> GetDoubleEma<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods)
        where TQuote : IQuote => quotes.GetDema(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename 'GetTripleEma(..)' to 'GetTema(..)' to fix.")]
    public static IEnumerable<TemaResult> GetTripleEma<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods)
        where TQuote : IQuote => quotes.GetTema(lookbackPeriods);
}

// RENAMED IN v1.23.0 - .ConvertToQuotes()
public static partial class Indicator
{
    [ExcludeFromCodeCoverage]
    [Obsolete("Rename 'ConvertToQuotes()' to 'ToQuotes()' to fix.")]
    public static IEnumerable<Quote> ConvertToQuotes(
        this IEnumerable<AdlResult> results) => results.ToQuotes();

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename 'ConvertToQuotes()' to 'ToQuotes()' to fix.")]
    public static IEnumerable<Quote> ConvertToQuotes(
        this IEnumerable<DpoResult> results) => results.ToQuotes();

    [ExcludeFromCodeCoverage]
    [Obsolete("'ConvertToQuotes()' is not needed for Heikin-Ashi.  Results can now be used directly as a replacement for IEnumerable<TQuote>.")]
    public static IEnumerable<Quote> ConvertToQuotes(
        this IEnumerable<HeikinAshiResult> results) => results
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

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename 'ConvertToQuotes()' to 'ToQuotes()' to fix.")]
    public static IEnumerable<Quote> ConvertToQuotes(
        this IEnumerable<HurstResult> results) => results.ToQuotes();

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename 'ConvertToQuotes()' to 'ToQuotes()' to fix.")]
    public static IEnumerable<Quote> ConvertToQuotes(
        this IEnumerable<ObvResult> results) => results.ToQuotes();

    [ExcludeFromCodeCoverage]
    [Obsolete("'ConvertToQuotes()' is not needed for Renko.  Results can now be used directly as a replacement for IEnumerable<TQuote>.")]
    public static IEnumerable<Quote> ConvertToQuotes(
        this IEnumerable<RenkoResult> results) => results
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

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename 'ConvertToQuotes()' to 'ToQuotes()' to fix.")]
    public static IEnumerable<Quote> ConvertToQuotes(
        this IEnumerable<RsiResult> results) => results.ToQuotes();
}

// REMOVED in v1.21.0
[ExcludeFromCodeCoverage]
[Obsolete("Using less than recommended quote history no longer throws an exception.  "
        + "See https://github.com/DaveSkender/Stock.Indicators/pull/685 for more info.")]
public class BadQuotesException : ArgumentOutOfRangeException
{
#nullable enable
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