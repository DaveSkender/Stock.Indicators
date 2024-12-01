using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

// ReSharper disable all
#pragma warning disable CS1591 // Missing XML comments

namespace Skender.Stock.Indicators;

// OBSOLETE IN v3.0.0
public static partial class Indicator
{
    // UTILITIES

    [ExcludeFromCodeCoverage]
    [Obsolete("This method no longer supports IEnumerable<TQuote> and tuple return types.", false)]
    public static IEnumerable<(DateTime Timestamp, double Value)> Use<TQuote>(
        this IEnumerable<TQuote> quotes,
            CandlePart candlePart = CandlePart.Close)
        where TQuote : IQuote
        => quotes.Use(candlePart);

    [ExcludeFromCodeCoverage]
    [Obsolete("This method no longer defaults to Close.  Rename Use() to Use(CandlePart.Close) for an explicit conversion.", false)]
    public static IEnumerable<(DateTime Timestamp, double Value)> Use<TQuote>(
        this IReadOnlyList<TQuote> quotes)
        where TQuote : IQuote
        => quotes.Select(x => (x.Timestamp, x.Value));

    [ExcludeFromCodeCoverage]
    [Obsolete("Refactor to use `ToSortedList()`", false)]
    public static Collection<TSeries> ToSortedCollection<TSeries>(
    this IEnumerable<TSeries> series)
    where TSeries : ISeries
        => series
            .OrderBy(x => x.Timestamp)
            .ToCollection();

    [ExcludeFromCodeCoverage]
    [Obsolete("Refactor to use `ToReusable()`", false)]
    public static Collection<(DateTime Timestamp, double Value)> ToTupleChainable<TResult>(
        this IEnumerable<TResult> reusable)
        where TResult : IReusable
        => reusable
            .Select(x => (x.Timestamp, x.Value))
            .OrderBy(x => x.Timestamp)
            .ToCollection();

    [ExcludeFromCodeCoverage]
    [Obsolete("Reference the `Value` property. Conversion is obsolete.", false)]
    public static Collection<(DateTime Date, double Value)> ToTupleNaN(
        this IEnumerable<IReusable> reusable)
        => reusable
            .ToSortedList()
            .Select(x => (x.Timestamp, x.Value))
            .ToCollection();

    [ExcludeFromCodeCoverage]
    [Obsolete("Refactor to use `ToReusable()`", false)]
    public static Collection<(DateTime Timestamp, double Value)> ToTupleCollection<TQuote>(
        this IEnumerable<TQuote> quotes,
        CandlePart candlePart = CandlePart.Close)
        where TQuote : IQuote
        => quotes
            .ToList()
            .ToReusable(candlePart)
            .Select(x => (x.Timestamp, x.Value))
            .ToCollection();

    [ExcludeFromCodeCoverage]
    [Obsolete("Refactor to use `List<TQuote> quotes` input.", false)]
    public static IReadOnlyList<TQuote> Validate<TQuote>(
        this IEnumerable<TQuote> quotes)
        where TQuote : IQuote
        => quotes.ToSortedList().Validate();

    [ExcludeFromCodeCoverage]
    [Obsolete("Refactor to use `List<TQuote> quotes` input.", false)]
    public static IEnumerable<Quote> Aggregate<TQuote>(
        this IEnumerable<TQuote> quotes, PeriodSize newSize)
        where TQuote : IQuote
        => quotes.ToSortedList().Aggregate(newSize);

    [ExcludeFromCodeCoverage]
    [Obsolete("Refactor to use `List<TQuote> quotes` input.", false)]
    public static IEnumerable<CandleProperties> ToCandles<TQuote>(
        this IEnumerable<TQuote> quotes)
        where TQuote : IQuote
        => quotes.ToSortedList().ToCandles();

    [ExcludeFromCodeCoverage]
    [Obsolete("Refactor to use `List.First(c => c.Timestamp == lookupDate)`", false)]
    public static TSeries Find<TSeries>(this IEnumerable<TSeries> series, DateTime lookupDate)
        where TSeries : ISeries => series.First(x => x.Timestamp == lookupDate);

    [ExcludeFromCodeCoverage]
    [Obsolete("Refactor to use `List.FindIndex(c => c.Timestamp == lookupDate)`", false)]
    public static int FindIndex<TSeries>(this List<TSeries> series, DateTime lookupDate)
        where TSeries : ISeries => series?.FindIndex(x => x.Timestamp == lookupDate) ?? -1;
}

// CLASSES AND INTERFACES

[Obsolete("Rename `IReusableResult` to `IReusable`", true)]
public interface IReusableResult : IReusable;

[ExcludeFromCodeCoverage]
[Obsolete("Rename `BasicData` to `QuotePart`", true)]
public sealed class BasicData : IReusable
{
    public DateTime Timestamp { get; set; }
    public double Value { get; set; }
}
