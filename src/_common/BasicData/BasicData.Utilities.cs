namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // convert to basic double class
    internal static List<BasicData> ToBasicClass<TQuote>(
        this IEnumerable<TQuote> quotes, CandlePart element = CandlePart.Close)
        where TQuote : IQuote
    {
        // elements represents the targeted candle parts, so use "O" to return <Open> as base data, etc.
        // convert to basic double precision format
        IEnumerable<BasicData> prices = element switch
        {
            CandlePart.Open => quotes.Select(x => new BasicData { Date = x.Date, Value = (double?)x.Open }),
            CandlePart.High => quotes.Select(x => new BasicData { Date = x.Date, Value = (double?)x.High }),
            CandlePart.Low => quotes.Select(x => new BasicData { Date = x.Date, Value = (double?)x.Low }),
            CandlePart.Close => quotes.Select(x => new BasicData { Date = x.Date, Value = (double?)x.Close }),
            CandlePart.Volume => quotes.Select(x => new BasicData { Date = x.Date, Value = (double?)x.Volume }),
            CandlePart.HL2 => quotes.Select(x => new BasicData { Date = x.Date, Value = (double?)(x.High + x.Low) / 2 }),
            CandlePart.HLC3 => quotes.Select(x => new BasicData { Date = x.Date, Value = (double?)(x.High + x.Low + x.Close) / 3 }),
            CandlePart.OC2 => quotes.Select(x => new BasicData { Date = x.Date, Value = (double?)(x.Open + x.Close) / 2 }),
            CandlePart.OHL3 => quotes.Select(x => new BasicData { Date = x.Date, Value = (double?)(x.Open + x.High + x.Low) / 3 }),
            CandlePart.OHLC4 => quotes.Select(x => new BasicData { Date = x.Date, Value = (double?)(x.Open + x.High + x.Low + x.Close) / 4 }),
            _ => new List<BasicData>()
        };

        return prices.OrderBy(x => x.Date).ToList();
    }

    // convert to basic double tuple
    internal static List<(DateTime Date, double? Value)> ToBasicTuple<TQuote>(
        this IEnumerable<TQuote> quotes, CandlePart element = CandlePart.Close)
        where TQuote : IQuote
    {
        // elements represents the targeted candle parts, so use "O" to return <Open> as base data, etc.
        // convert to basic double precision format (Close is default)
        IEnumerable<(DateTime Date, double? Value)> prices = element switch
        {
            CandlePart.Open => quotes.Select(x => (x.Date, (double?)x.Open)),
            CandlePart.High => quotes.Select(x => (x.Date, (double?)x.High)),
            CandlePart.Low => quotes.Select(x => (x.Date, (double?)x.Low)),
            CandlePart.Close => quotes.Select(x => (x.Date, (double?)x.Close)),
            CandlePart.Volume => quotes.Select(x => (x.Date, (double?)x.Volume)),
            CandlePart.HL2 => quotes.Select(x => (x.Date, (double?)(x.High + x.Low) / 2)),
            CandlePart.HLC3 => quotes.Select(x => (x.Date, (double?)(x.High + x.Low + x.Close) / 3)),
            CandlePart.OC2 => quotes.Select(x => (x.Date, (double?)(x.Open + x.Close) / 2)),
            CandlePart.OHL3 => quotes.Select(x => (x.Date, (double?)(x.Open + x.High + x.Low) / 3)),
            CandlePart.OHLC4 => quotes.Select(x => (x.Date, (double?)(x.Open + x.High + x.Low + x.Close) / 4)),
            _ => quotes.Select(x => (x.Date, (double?)x.Close))
        };

        return prices.OrderBy(x => x.Date).ToList();
    }

    // sort basic double class
    internal static List<IBasicData> SortToList(
        this IEnumerable<IBasicData> basicData)
        => basicData
            .Condense()
            .OrderBy(x => x.Date)
            .ToList();

    // remove initial null periods
    internal static List<IBasicData> Condense(
        this IEnumerable<IBasicData> baseData)
    {
        int removePeriods = baseData
            .ToList()
            .FindIndex(x => x.Value != null);

        return baseData.Remove(removePeriods);
    }

    // remove specific periods
    private static List<IBasicData> Remove(
        this IEnumerable<IBasicData> baseData,
        int removePeriods)
    {
        List<IBasicData> resultsList = baseData.ToList();

        if (resultsList.Count <= removePeriods)
        {
            return new List<IBasicData>();
        }
        else
        {
            if (removePeriods > 0)
            {
                for (int i = 0; i < removePeriods; i++)
                {
                    resultsList.RemoveAt(0);
                }
            }

            return resultsList;
        }
    }
}