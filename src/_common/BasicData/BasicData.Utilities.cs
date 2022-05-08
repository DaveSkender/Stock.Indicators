namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // convert to basic double class
    internal static List<BasicData> ToBasicClass<TQuote>(
        this IEnumerable<TQuote> quotes, CandlePart element = CandlePart.Close)
        where TQuote : IQuote
    {
        IEnumerable<BasicData> prices = element switch
        {
            CandlePart.Open => quotes.Select(x => new BasicData { Date = x.Date, Value = (double)x.Open }),
            CandlePart.High => quotes.Select(x => new BasicData { Date = x.Date, Value = (double)x.High }),
            CandlePart.Low => quotes.Select(x => new BasicData { Date = x.Date, Value = (double)x.Low }),
            CandlePart.Close => quotes.Select(x => new BasicData { Date = x.Date, Value = (double)x.Close }),
            CandlePart.Volume => quotes.Select(x => new BasicData { Date = x.Date, Value = (double)x.Volume }),
            CandlePart.HL2 => quotes.Select(x => new BasicData { Date = x.Date, Value = (double)(x.High + x.Low) / 2 }),
            CandlePart.HLC3 => quotes.Select(x => new BasicData { Date = x.Date, Value = (double)(x.High + x.Low + x.Close) / 3 }),
            CandlePart.OC2 => quotes.Select(x => new BasicData { Date = x.Date, Value = (double)(x.Open + x.Close) / 2 }),
            CandlePart.OHL3 => quotes.Select(x => new BasicData { Date = x.Date, Value = (double)(x.Open + x.High + x.Low) / 3 }),
            CandlePart.OHLC4 => quotes.Select(x => new BasicData { Date = x.Date, Value = (double)(x.Open + x.High + x.Low + x.Close) / 4 }),
            _ => new List<BasicData>()
        };

        return prices.OrderBy(x => x.Date).ToList();
    }

    // convert to basic double tuple
    internal static List<(DateTime Date, double Value)> ToBasicTuple<TQuote>(
        this IEnumerable<TQuote> quotes, CandlePart element = CandlePart.Close)
        where TQuote : IQuote
    {
        IEnumerable<(DateTime Date, double Value)> prices = element switch
        {
            CandlePart.Open => quotes.Select(x => (x.Date, (double)x.Open)),
            CandlePart.High => quotes.Select(x => (x.Date, (double)x.High)),
            CandlePart.Low => quotes.Select(x => (x.Date, (double)x.Low)),
            CandlePart.Close => quotes.Select(x => (x.Date, (double)x.Close)),
            CandlePart.Volume => quotes.Select(x => (x.Date, (double)x.Volume)),
            CandlePart.HL2 => quotes.Select(x => (x.Date, (double)(x.High + x.Low) / 2)),
            CandlePart.HLC3 => quotes.Select(x => (x.Date, (double)(x.High + x.Low + x.Close) / 3)),
            CandlePart.OC2 => quotes.Select(x => (x.Date, (double)(x.Open + x.Close) / 2)),
            CandlePart.OHL3 => quotes.Select(x => (x.Date, (double)(x.Open + x.High + x.Low) / 3)),
            CandlePart.OHLC4 => quotes.Select(x => (x.Date, (double)(x.Open + x.High + x.Low + x.Close) / 4)),
            _ => new List<(DateTime, double)>()
        };

        return prices.OrderBy(x => x.Date).ToList();
    }

    // convert basic data to tuple
    internal static List<(DateTime Date, double Value)> ToResultTuple(
        this IEnumerable<IReusableResult> basicData)
    {
#pragma warning disable CS8629 // Nullable value type may be null.  False warning.
        IEnumerable<(DateTime Date, double)> prices = basicData
            .Where(x => x.Value is not null)
            .Select(x => (x.Date, (double)x.Value));
#pragma warning restore CS8629

        return prices.OrderBy(x => x.Date).ToList();
    }
}