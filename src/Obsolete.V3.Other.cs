using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace Skender.Stock.Indicators;
#pragma warning disable CS1591 // Missing XML comments

/// <summary>
/// OBSOLETE IN v3.0.0
/// </summary>
[ExcludeFromCodeCoverage]
[Obsolete("The broad Indicator class has been replaced by specific indicator classes.", true)]
public static partial class Indicator
{
    // UTILITIES

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
    [Obsolete("This method no longer defaults to Close.  Rename Use() to Use(CandlePart.Close) for an explicit conversion.", false)]
    public static IEnumerable<(DateTime Timestamp, double Value)> Use(
        this IReadOnlyList<IQuote> quotes)
        => quotes.Select(static x => (x.Timestamp, x.Value));

    [ExcludeFromCodeCoverage]
    [Obsolete("Refactor to use `ToSortedList()`", false)]
    public static Collection<TSeries> ToSortedCollection<TSeries>(
    this IEnumerable<TSeries> series)
    where TSeries : ISeries
        => series
            .OrderBy(static x => x.Timestamp)
            .ToCollection();

    [ExcludeFromCodeCoverage]
    [Obsolete("Refactor to use `ToReusable()`", false)]
    public static Collection<(DateTime Timestamp, double Value)> ToTupleChainable<TResult>(
        this IEnumerable<TResult> reusable)
        where TResult : IReusable
        => reusable
            .Select(static x => (x.Timestamp, x.Value))
            .OrderBy(static x => x.Timestamp)
            .ToCollection();

    [ExcludeFromCodeCoverage]
    [Obsolete("Reference the `Value` property. Conversion is obsolete.", false)]
    public static Collection<(DateTime Date, double Value)> ToTupleNaN(
        this IEnumerable<IReusable> reusable)
        => reusable
            .ToSortedList()
            .Select(static x => (x.Timestamp, x.Value))
            .ToCollection();

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

    [ExcludeFromCodeCoverage]
    [Obsolete("Refactor to use `List<TQuote> quotes` input.", false)]
    public static IReadOnlyList<TQuote> Validate<TQuote>(
        this IEnumerable<TQuote> quotes)
        where TQuote : IQuote
        => quotes.ToSortedList().Validate();

    [ExcludeFromCodeCoverage]
    [Obsolete("Refactor to use `List<TQuote> quotes` input.", false)]
    public static IEnumerable<Quote> Aggregate(
        this IEnumerable<IQuote> quotes, PeriodSize newSize)
        => quotes.ToSortedList().Aggregate(newSize);

    [ExcludeFromCodeCoverage]
    [Obsolete("Refactor to use `List<TQuote> quotes` input.", false)]
    public static IEnumerable<CandleProperties> ToCandles(
        this IEnumerable<IQuote> quotes)
        => quotes.ToSortedList().ToCandles();

    [ExcludeFromCodeCoverage]
    [Obsolete("Refactor to use `List.First(c => c.Timestamp == lookupDate)`", false)]
    public static TSeries Find<TSeries>(this IEnumerable<TSeries> series, DateTime lookupDate)
        where TSeries : ISeries => series.First(x => x.Timestamp == lookupDate);

    [ExcludeFromCodeCoverage]
    [Obsolete("Refactor to use `List.FindIndex(c => c.Timestamp == lookupDate)`", false)]
    public static int FindIndex<TSeries>(this List<TSeries> series, DateTime lookupDate)
        where TSeries : ISeries => series?.FindIndex(x => x.Timestamp == lookupDate) ?? -1;

    /// <summary>
    /// Rounds a nullable decimal value to a specified number of fractional digits.
    /// </summary>
    /// <param name="value">The nullable decimal value.</param>
    /// <param name="digits">The number of fractional digits.</param>
    /// <returns>The rounded value, or null if the input is null.</returns>
    [Obsolete("Rounding deprecated.  Use Math.Round() instead.")]
    [ExcludeFromCodeCoverage]
    public static decimal? Round(this decimal? value, int digits)
        => value.HasValue
        ? Math.Round(value.GetValueOrDefault(), digits)
        : null;

    /// <summary>
    /// Rounds a nullable double value to a specified number of fractional digits.
    /// </summary>
    /// <param name="value">The nullable double value.</param>
    /// <param name="digits">The number of fractional digits.</param>
    /// <returns>The rounded value, or null if the input is null.</returns>
    [ExcludeFromCodeCoverage]
    [Obsolete("Rounding deprecated.  Use Math.Round() instead.")]
    public static double? Round(this double? value, int digits)
        => value.HasValue
        ? Math.Round(value.GetValueOrDefault(), digits)
        : null;

    /// <summary>
    /// Rounds a double value to a specified number of fractional digits.
    /// It is an extension alias of <see cref="Math.Round(double, int)"/>
    /// </summary>
    /// <param name="value">The double value.</param>
    /// <param name="digits">The number of fractional digits.</param>
    /// <returns>The rounded value.</returns>
    [ExcludeFromCodeCoverage]
    [Obsolete("Rounding deprecated.  Use Math.Round() instead.")]
    public static double Round(this double value, int digits)
        => Math.Round(value, digits);

    /// <summary>
    /// Rounds a decimal value to a specified number of fractional digits.
    /// It is an extension alias of <see cref="Math.Round(decimal, int)"/>
    /// </summary>
    /// <param name="value">The decimal value.</param>
    /// <param name="digits">The number of fractional digits.</param>
    /// <returns>The rounded value.</returns>
    [ExcludeFromCodeCoverage]
    [Obsolete("Rounding deprecated.  Use Math.Round() instead.")]
    public static decimal Round(this decimal value, int digits)
        => Math.Round(value, digits);
}

// CLASSES AND INTERFACES

[Obsolete("Rename `IReusableResult` to `IReusable`", true)]
public interface IReusableResult : IReusable;

[ExcludeFromCodeCoverage]
[Obsolete("Rename `BasicData` to `QuotePart`", true)]
public sealed class BasicData : IReusable
{
    public DateTime Timestamp { get; set; }

    [JsonIgnore]
    public double Value { get; set; }
}

/// <summary>
/// ENUM
/// </summary>
[Obsolete($"Rename '{nameof(ChandelierType)}' to '{nameof(Direction)}'.")]
[SuppressMessage("Naming", "CA1720:Identifier contains type name", Justification = "Not really an issue.")]
public enum ChandelierType
{
    Long = 0,
    Short = 1
}
