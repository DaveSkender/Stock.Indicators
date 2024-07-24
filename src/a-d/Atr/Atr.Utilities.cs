namespace Skender.Stock.Indicators;

public static partial class Atr
{
    // increment
    public static AtrResult Increment<TQuote>(
        int lookbackPeriods,
        TQuote quote,
        double prevClose,
        double? prevAtr)
        where TQuote : IQuote
    {
        double high = (double)quote.High;
        double low = (double)quote.Low;
        double close = (double)quote.Close;

        double? tr = Tr.Increment(high, low, prevClose).NaN2Null();
        double? atr = ((prevAtr * (lookbackPeriods - 1)) + tr) / lookbackPeriods;
        double? atrp = close == 0 ? null : atr / close * 100;

        return new AtrResult(quote.Timestamp, tr, atr, atrp);
    }

    // remove recommended periods
    public static IReadOnlyList<AtrResult> RemoveWarmupPeriods(
        this IEnumerable<AtrResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.Atr != null);

        return results.Remove(removePeriods);
    }

    // parameter validation
    internal static void Validate(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 1)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 1 for Average True Range.");
        }
    }
}
