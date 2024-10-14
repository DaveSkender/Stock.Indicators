namespace Skender.Stock.Indicators;

// QUOTE UTILITIES

public static partial class Quotes
{
    // aggregation (quantization)
    /// <include file='./info.xml' path='info/type[@name="Aggregate"]/*' />
    ///
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

    // aggregation (quantization) using TimeSpan
    /// <include file='./info.xml' path='info/type[@name="AggregateTimeSpan"]/*' />
    ///
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
