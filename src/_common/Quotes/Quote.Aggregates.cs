namespace Skender.Stock.Indicators;

// HISTORICAL QUOTES FUNCTIONS (AGGREGATES)

public static partial class QuoteUtility
{
    // aggregation (quantization)
    /// <include file='./info.xml' path='info/type[@name="Aggregate"]/*' />
    ///
    public static IEnumerable<Quote> Aggregate<TQuote>(
        this IEnumerable<TQuote> quotes,
        PeriodSize newSize)
        where TQuote : IQuote
    {
        if (newSize != PeriodSize.Month)
        {
            // parameter conversion
            TimeSpan newTimeSpan = newSize.ToTimeSpan();

            // convert
            return quotes.Aggregate(newTimeSpan);
        }
        else // month
        {
            return quotes
            .OrderBy(x => x.Date)
            .GroupBy(x => new DateTime(x.Date.Year, x.Date.Month, 1))
            .Select(x => new Quote
            {
                Date = x.Key,
                Open = x.First().Open,
                High = x.Max(t => t.High),
                Low = x.Min(t => t.Low),
                Close = x.Last().Close,
                Volume = x.Sum(t => t.Volume)
            });
        }
    }

    // aggregation (quantization) using TimeSpan
    /// <include file='./info.xml' path='info/type[@name="AggregateTimeSpan"]/*' />
    ///
    public static IEnumerable<Quote> Aggregate<TQuote>(
        this IEnumerable<TQuote> quotes,
        TimeSpan timeSpan)
        where TQuote : IQuote
    {
        // handle no quotes scenario
        if (quotes == null || !quotes.Any())
        {
            return new List<Quote>();
        }

        if (timeSpan <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(timeSpan), timeSpan,
                "Quotes Aggregation must use a usable new size value (see documentation for options).");
        }

        // return aggregation
        return quotes
            .OrderBy(x => x.Date)
            .GroupBy(x => x.Date.RoundDown(timeSpan))
            .Select(x => new Quote
            {
                Date = x.Key,
                Open = x.First().Open,
                High = x.Max(t => t.High),
                Low = x.Min(t => t.Low),
                Close = x.Last().Close,
                Volume = x.Sum(t => t.Volume)
            });
    }
}
