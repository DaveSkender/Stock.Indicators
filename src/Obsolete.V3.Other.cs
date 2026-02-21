using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace Skender.Stock.Indicators;

/// <summary>
/// OBSOLETE IN v3.0.0
/// </summary>
[ExcludeFromCodeCoverage]
[Obsolete("The broad Indicator class has been replaced by specific indicator classes.", true)]
public static partial class Indicator
{
    // UTILITIES

    /// <summary>Obsolete. This method no longer supports IEnumerable&lt;IQuote&gt; and tuple return types.</summary>
    [ExcludeFromCodeCoverage]
    [Obsolete("This method no longer supports IEnumerable<IQuote> and tuple return types.", false)]
    public static IEnumerable<(DateTime Timestamp, double Value)> Use(
        this IEnumerable<IQuote> quotes,
            CandlePart candlePart = CandlePart.Close)
        => quotes
            .ToSortedList()
            .ToReusable(candlePart)
            .Select(static x => (x.Timestamp, x.Value));

    [ExcludeFromCodeCoverage]
    [Obsolete("Use 'Collection<T> Foo = [..myEnumerable];' instead.")]
    internal static Collection<T> ToCollection<T>(this IEnumerable<T> source) => [.. source];

    /// <summary>Obsolete. Rename Use() to Use(CandlePart.Close) for an explicit conversion.</summary>
    [Obsolete("This method no longer defaults to Close.  Rename Use() to Use(CandlePart.Close) for an explicit conversion.", false)]
    public static IEnumerable<(DateTime Timestamp, double Value)> Use(
        this IReadOnlyList<IQuote> quotes)
        => quotes.Select(static x => (x.Timestamp, x.Value));

    /// <summary>Obsolete. Refactor to use ToSortedList().</summary>
    [ExcludeFromCodeCoverage]
    [Obsolete("Refactor to use `ToSortedList()`", false)]
    public static Collection<TSeries> ToSortedCollection<TSeries>(
    this IEnumerable<TSeries> series)
    where TSeries : ISeries
        => series
            .OrderBy(static x => x.Timestamp)
            .ToCollection();

    /// <summary>Obsolete. Refactor to use ToReusable().</summary>
    [ExcludeFromCodeCoverage]
    [Obsolete("Refactor to use `ToReusable()`", false)]
    public static Collection<(DateTime Timestamp, double Value)> ToTupleChainable<TResult>(
        this IEnumerable<TResult> reusable)
        where TResult : IReusable
        => reusable
            .Select(static x => (x.Timestamp, x.Value))
            .OrderBy(static x => x.Timestamp)
            .ToCollection();

    /// <summary>Obsolete. Reference the Value property instead.</summary>
    [ExcludeFromCodeCoverage]
    [Obsolete("Reference the `Value` property. Conversion is obsolete.", false)]
    public static Collection<(DateTime Date, double Value)> ToTupleNaN(
        this IEnumerable<IReusable> reusable)
        => reusable
            .ToSortedList()
            .Select(static x => (x.Timestamp, x.Value))
            .ToCollection();

    /// <summary>Obsolete. Refactor to use ToReusable().</summary>
    [ExcludeFromCodeCoverage]
    [Obsolete("Refactor to use `ToReusable()`", false)]
    public static Collection<(DateTime Timestamp, double Value)> ToTupleCollection(
        this IEnumerable<IQuote> quotes,
        CandlePart candlePart = CandlePart.Close)
        => quotes
            .ToList()
            .ToReusable(candlePart)
            .Select(static x => (x.Timestamp, x.Value))
            .ToCollection();

    /// <summary>Obsolete. Refactor to use List&lt;TQuote&gt; quotes input.</summary>
    [ExcludeFromCodeCoverage]
    [Obsolete("Refactor to use `List<TQuote> quotes` input.", false)]
    public static IReadOnlyList<TQuote> Validate<TQuote>(
        this IEnumerable<TQuote> quotes)
        where TQuote : IQuote
        => quotes.ToSortedList().Validate();

    /// <summary>Obsolete. Refactor to use List&lt;TQuote&gt; quotes input.</summary>
    [ExcludeFromCodeCoverage]
    [Obsolete("Refactor to use `List<TQuote> quotes` input.", false)]
    public static IEnumerable<Quote> Aggregate(
        this IEnumerable<IQuote> quotes, PeriodSize newSize)
        => quotes.ToSortedList().Aggregate(newSize);

    /// <summary>Obsolete. Refactor to use List&lt;TQuote&gt; quotes input.</summary>
    [ExcludeFromCodeCoverage]
    [Obsolete("Refactor to use `List<TQuote> quotes` input.", false)]
    public static IEnumerable<CandleProperties> ToCandles(
        this IEnumerable<IQuote> quotes)
        => quotes.ToSortedList().ToCandles();

    /// <summary>Obsolete. Refactor to use List.First(c => c.Timestamp == lookupDate).</summary>
    [ExcludeFromCodeCoverage]
    [Obsolete("Refactor to use `List.First(c => c.Timestamp == lookupDate)`", false)]
    public static TSeries Find<TSeries>(this IEnumerable<TSeries> series, DateTime lookupDate)
        where TSeries : ISeries => series.First(x => x.Timestamp == lookupDate);

    /// <summary>Obsolete. Refactor to use List.FindIndex(c => c.Timestamp == lookupDate).</summary>
    [ExcludeFromCodeCoverage]
    [Obsolete("Refactor to use `List.FindIndex(c => c.Timestamp == lookupDate)`", false)]
    public static int FindIndex<TSeries>(this List<TSeries> series, DateTime lookupDate)
        where TSeries : ISeries => series?.FindIndex(x => x.Timestamp == lookupDate) ?? -1;

    /// <summary>Obsolete. Use Math.Round() instead.</summary>
    [Obsolete("Rounding deprecated.  Use Math.Round() instead.")]
    [ExcludeFromCodeCoverage]
    public static decimal? Round(this decimal? value, int digits)
        => value.HasValue
        ? Math.Round(value.GetValueOrDefault(), digits)
        : null;

    /// <summary>Obsolete. Use Math.Round() instead.</summary>
    [ExcludeFromCodeCoverage]
    [Obsolete("Rounding deprecated.  Use Math.Round() instead.")]
    public static double? Round(this double? value, int digits)
        => value.HasValue
        ? Math.Round(value.GetValueOrDefault(), digits)
        : null;

    /// <summary>Obsolete. Use Math.Round() instead.</summary>
    [ExcludeFromCodeCoverage]
    [Obsolete("Rounding deprecated.  Use Math.Round() instead.")]
    public static double Round(this double value, int digits)
        => Math.Round(value, digits);

    /// <summary>Obsolete. Use Math.Round() instead.</summary>
    [ExcludeFromCodeCoverage]
    [Obsolete("Rounding deprecated.  Use Math.Round() instead.")]
    public static decimal Round(this decimal value, int digits)
        => Math.Round(value, digits);
}

// CLASSES AND INTERFACES

/// <summary>Obsolete. Rename IReusableResult to IReusable.</summary>
[Obsolete("Rename `IReusableResult` to `IReusable`", true)]
public interface IReusableResult : IReusable;

/// <summary>Obsolete. Rename BasicData to TimeValue.</summary>
[ExcludeFromCodeCoverage]
[Obsolete("Rename `BasicData` to `TimeValue`", true)]
public sealed class BasicData : IReusable
{
    /// <inheritdoc />
    public DateTime Timestamp { get; set; }

    /// <inheritdoc />
    [JsonIgnore]
    public double Value { get; set; }
}

/// <summary>Obsolete. Rename ChandelierType to Direction.</summary>
[Obsolete($"Rename '{nameof(ChandelierType)}' to '{nameof(Direction)}'.")]
[SuppressMessage("Naming", "CA1720:Identifier contains type name", Justification = "Not really an issue.")]
public enum ChandelierType
{
    /// <summary>Obsolete. Long direction.</summary>
    Long = 0,

    /// <summary>Obsolete. Short direction.</summary>
    Short = 1
}
