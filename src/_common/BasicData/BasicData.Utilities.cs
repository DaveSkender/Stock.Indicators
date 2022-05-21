namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // convert to basic double class
    internal static List<BasicData> ToBasicClass<TQuote>(
        this IEnumerable<TQuote> quotes, CandlePart candlePart = CandlePart.Close)
        where TQuote : IQuote
    {
        IEnumerable<BasicData> prices = candlePart switch
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
            _ => throw new ArgumentOutOfRangeException(
                nameof(candlePart), candlePart, "Invalid candlePart provided."),
        };

        return prices.OrderBy(x => x.Date).ToList();
    }

    // convert result to tuple
    internal static List<(DateTime Date, double Value)> ToResultTuple(
        this IEnumerable<IReusableResult> basicData)
    {
        List<(DateTime Date, double Value)> prices = new();
        List<IReusableResult>? bdList = basicData.ToList();

        // find first non-nulled
        int first = bdList.FindIndex(x => x.Value != null);

        for (int i = first; i < bdList.Count; i++)
        {
            IReusableResult? q = bdList[i];
            double value = (q.Value == null) ? double.NaN : (double)q.Value;
            prices.Add(new(q.Date, value));
        }

        return prices.OrderBy(x => x.Date).ToList();
    }
}