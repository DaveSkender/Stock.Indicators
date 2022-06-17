namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // STOCHASTIC RSI

    /// <include file='./info.xml' path='indicator/*' />
    ///
    public static IEnumerable<StochRsiResult> GetStochRsi<TQuote>(
       this IEnumerable<TQuote> quotes,
       int rsiPeriods,
       int stochPeriods,
       int signalPeriods,
       int smoothPeriods = 1,
       double kFactor = 3,
       double dFactor = 2,
       MaType movingAverageType = MaType.SMA)
       where TQuote : IQuote
    {
        // check parameter arguments
        ValidateStochRsi(rsiPeriods, stochPeriods, signalPeriods, smoothPeriods);

        // initialize
        return GetStochRsi(GetRsi(quotes, rsiPeriods), rsiPeriods, stochPeriods, signalPeriods, smoothPeriods, kFactor, dFactor, movingAverageType).ToList();
    }

    public static IEnumerable<StochRsiResult> GetStochRsi(
    this IEnumerable<RsiResult> rsiResults,
    int rsiPeriods,
    int stochPeriods,
    int signalPeriods,
    int smoothPeriods = 1,
    double kFactor = 3,
    double dFactor = 2,
    MaType movingAverageType = MaType.SMA)
    {
        List<StochRsiResult> results = new(rsiResults.Count());

        // get Stochastic of RSI
#pragma warning disable CS8629 // Nullable value type may be null.  False warning.
        List<StochResult> stoResults =
            rsiResults
            .Remove(rsiPeriods)
            .Where(x => x.Rsi is not null)
            .Select(x => new Quote
            {
                Date = x.Date,
                High = (decimal)x.Rsi,
                Low = (decimal)x.Rsi,
                Close = (decimal)x.Rsi
            })
            .GetStoch(stochPeriods, signalPeriods, smoothPeriods, kFactor, dFactor, movingAverageType)
            .ToList();
#pragma warning restore CS8629

        // compose
        for (int i = 0; i < rsiResults.Count(); i++)
        {
            RsiResult r = rsiResults.ElementAt(i);

            StochRsiResult result = new()
            {
                Date = r.Date
            };

            StochResult sto;
            if (i + 1 >= rsiPeriods + stochPeriods)
            {
                sto = stoResults[i - rsiPeriods];
                result.StochRsi = sto.Oscillator;
                result.Signal = sto.Signal;
            }

            results.Add(result);
        }

        return results;
    }

    // parameter validation
    private static void ValidateStochRsi(
        int rsiPeriods,
        int stochPeriods,
        int signalPeriods,
        int smoothPeriods)
    {
        // check parameter arguments
        if (rsiPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(rsiPeriods), rsiPeriods,
                "RSI periods must be greater than 0 for Stochastic RSI.");
        }

        if (stochPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(stochPeriods), stochPeriods,
                "STOCH periods must be greater than 0 for Stochastic RSI.");
        }

        if (signalPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(signalPeriods), signalPeriods,
                "Signal periods must be greater than 0 for Stochastic RSI.");
        }

        if (smoothPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(smoothPeriods), smoothPeriods,
                "Smooth periods must be greater than 0 for Stochastic RSI.");
        }
    }
}
