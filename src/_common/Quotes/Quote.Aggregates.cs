// QUOTE UTILITIES

public static partial class Quotes
{
    /// <summary>
    /// Aggregates the quotes to a specified period size.
    /// </summary>
    /// <typeparam name="TQuote">Type of the quotes, must implement IQuote.</typeparam>
    /// <param name="quotes">The quotes to aggregate.</param>
    /// <param name="newSize">The new period size to aggregate to.</param>
    /// <returns>A list of aggregated quotes.</returns>
    public static IReadOnlyList<Quote> Aggregate<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        PeriodSize newSize)
        where TQuote : IQuote
    {
        if (newSize == PeriodSize.Month)
        {
            return quotes
                .OrderBy(x => x.Timestamp)
                .GroupBy(x => new DateTime(x.Timestamp.Year, x.Timestamp.Month, 1))
                .Select(x => new Quote(
                    Timestamp: x.Key,
                    Open: x.First().Open,
                    High: x.Max(t => t.High),
                    Low: x.Min(t => t.Low),
                    Close: x.Last().Close,
                    Volume: x.Sum(t => t.Volume)))
                .ToList();
        }

        // parameter conversion
        TimeSpan newTimeSpan = newSize.ToTimeSpan();

        // convert
        return quotes.Aggregate(newTimeSpan);

        // month
    }

    /// <summary>
    /// Aggregates the quotes to a specified time span.
    /// </summary>
    /// <typeparam name="TQuote">Type of the quotes, must implement IQuote.</typeparam>
    /// <param name="quotes">The quotes to aggregate.</param>
    /// <param name="timeSpan">The time span to aggregate to.</param>
    /// <returns>A list of aggregated quotes.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the time span is less than or equal to zero.</exception>
    public static IReadOnlyList<Quote> Aggregate<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        TimeSpan timeSpan)
        where TQuote : IQuote
    {
        if (timeSpan <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(timeSpan), timeSpan,
                "Quotes Aggregation must use a usable new size value (see documentation for options).");
        }

        // return aggregation
        return quotes
            .OrderBy(x => x.Timestamp)
            .GroupBy(x => x.Timestamp.RoundDown(timeSpan))
            .Select(x => new Quote(
                Timestamp: x.Key,
                Open: x.First().Open,
                High: x.Max(t => t.High),
                Low: x.Min(t => t.Low),
                Close: x.Last().Close,
                Volume: x.Sum(t => t.Volume)))
            .ToList();
    }
}
