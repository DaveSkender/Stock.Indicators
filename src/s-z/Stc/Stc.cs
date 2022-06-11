namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // SCHAFF TREND CYCLE (STC)
    /// <include file='./info.xml' path='indicator/*' />
    ///
    public static IEnumerable<StcResult> GetStc<TQuote>(
        this IEnumerable<TQuote> quotes,
        int cyclePeriods = 10,
        int fastPeriods = 23,
        int slowPeriods = 50)
        where TQuote : IQuote
    {
        // convert quotes
        List<BasicData> quotesList = quotes.ToBasicClass(CandlePart.Close);

        // check parameter arguments
        ValidateStc(cyclePeriods, fastPeriods, slowPeriods);

        // get stochastic of macd
        // TODO: make this work better
        IEnumerable<StochResult> stochMacd = quotes
          .GetMacd(fastPeriods, slowPeriods, 1)
          .Remove(slowPeriods - 1)
          .Where(x => x.Macd is not null)
          .Select(x => new Quote
          {
              Date = x.Date,
              High = (decimal)x.Macd,
              Low = (decimal)x.Macd,
              Close = (decimal)x.Macd
          })
          .GetStoch(cyclePeriods, 1, 3);

        // initialize results
        // to ensure same length as original quotes
        int length = quotesList.Count;
        int initPeriods = Math.Min(slowPeriods - 1, length);
        List<StcResult> results = new(length);

        for (int i = 0; i < initPeriods; i++)
        {
            BasicData q = quotesList[i];
            results.Add(new StcResult() { Date = q.Date });
        }

        // add stoch results
        // TODO: see if List Add works faster
        results.AddRange(
           stochMacd
           .Select(x => new StcResult
           {
               Date = x.Date,
               Stc = x.Oscillator
           }));

        return results;
    }

    // parameter validation
    private static void ValidateStc(
        int cyclePeriods,
        int fastPeriods,
        int slowPeriods)
    {
        // check parameter arguments
        if (cyclePeriods < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(cyclePeriods), cyclePeriods,
                "Trend Cycle periods must be greater than or equal to 0 for STC.");
        }

        if (fastPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(fastPeriods), fastPeriods,
                "Fast periods must be greater than 0 for STC.");
        }

        if (slowPeriods <= fastPeriods)
        {
            throw new ArgumentOutOfRangeException(nameof(slowPeriods), slowPeriods,
                "Slow periods must be greater than the fast period for STC.");
        }
    }
}
