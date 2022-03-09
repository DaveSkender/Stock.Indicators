namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // EXPONENTIAL MOVING AVERAGE (on CLOSE price)
    /// <include file='./info.xml' path='indicator/type[@name="Main"]/*' />
    ///
    public static IEnumerable<EmaResult> GetEma<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods)
        where TQuote : IQuote
    {
        // convert quotes
        List<BasicD> bdList = quotes.ConvertToBasic(CandlePart.Close);

        // calculate
        return bdList.CalcEma(lookbackPeriods);
    }

    // EXPONENTIAL MOVING AVERAGE (on specified OHLCV part)
    /// <include file='./info.xml' path='indicator/type[@name="Custom"]/*' />
    ///
    public static IEnumerable<EmaResult> GetEma<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods,
        CandlePart candlePart)
        where TQuote : IQuote
    {
        // convert quotes
        List<BasicD> bdList = quotes.ConvertToBasic(candlePart);

        // calculate
        return bdList.CalcEma(lookbackPeriods);
    }

    // standard calculation
    private static List<EmaResult> CalcEma(
        this List<BasicD> bdList, int lookbackPeriods)
    {
        // check parameter arguments
        ValidateEma(lookbackPeriods);

        // initialize
        int length = bdList.Count;
        List<EmaResult> results = new(length);

        double k = 2d / (lookbackPeriods + 1);
        double? lastEma = 0;
        int initPeriods = Math.Min(lookbackPeriods, length);

        for (int i = 0; i < initPeriods; i++)
        {
            lastEma += bdList[i].Value;
        }

        lastEma /= lookbackPeriods;

        // roll through quotes
        for (int i = 0; i < length; i++)
        {
            BasicD h = bdList[i];
            int index = i + 1;

            EmaResult result = new()
            {
                Date = h.Date
            };

            if (index > lookbackPeriods)
            {
                double? ema = lastEma + (k * (h.Value - lastEma));
                result.Ema = (decimal?)ema;
                lastEma = ema;
            }
            else if (index == lookbackPeriods)
            {
                result.Ema = (decimal?)lastEma;
            }

            results.Add(result);
        }

        return results;
    }

    // parameter validation
    private static void ValidateEma(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for EMA.");
        }
    }
}
