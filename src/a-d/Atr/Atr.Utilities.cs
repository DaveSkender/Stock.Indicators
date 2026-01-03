namespace Skender.Stock.Indicators;

public static partial class Atr
{
    /// <summary>
    /// Calculates the Average True Range (ATR) incrementally.
    /// </summary>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <param name="high">The high price of the current period.</param>
    /// <param name="low">The low price of the current period.</param>
    /// <param name="prevClose">The close price of the previous period.</param>
    /// <param name="prevAtr">The ATR value of the previous period.</param>
    /// <returns>The ATR value for the current period.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Increment(
        int lookbackPeriods,
        double high,
        double low,
        double prevClose,
        double prevAtr)
    {
        double tr = Tr.Increment(high, low, prevClose);
        return ((prevAtr * (lookbackPeriods - 1)) + tr) / lookbackPeriods;
    }

    /// <summary>
    /// Calculates the Average True Range (ATR) incrementally for a given quote.
    /// </summary>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <param name="quote">The current quote.</param>
    /// <param name="prevClose">The close price of the previous period.</param>
    /// <param name="prevAtr">The ATR value of the previous period.</param>
    /// <returns>An <see cref="AtrResult"/> containing the ATR values for the current period.</returns>
    public static AtrResult Increment(
        int lookbackPeriods,
        IQuote quote,
        double prevClose,
        double? prevAtr)
    {
        ArgumentNullException.ThrowIfNull(quote);

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

    /// <summary>
    /// Validates the parameters for the Average True Range (ATR) calculation.
    /// </summary>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are less than or equal to 1.</exception>
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
