namespace Skender.Stock.Indicators;

public static partial class Atr
{
    // increment
    internal static double Increment(
        int lookbackPeriods,
        double high,
        double low,
        double prevClose,
        double prevAtr)
    {
        double tr = Tr.Increment(high, low, prevClose);
        return ((prevAtr * (lookbackPeriods - 1)) + tr) / lookbackPeriods;

        // TODO: this may be unused, verify before making public
    }

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

        double tr = Tr.Increment(high, low, prevClose);
        double atr = (((prevAtr ?? double.NaN) * (lookbackPeriods - 1)) + tr) / lookbackPeriods;
        double atrp = close == 0 ? double.NaN : atr / close * 100;

        return new AtrResult(
            quote.Timestamp,
            tr,
            atr.NaN2Null(),
            atrp.NaN2Null());
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
