namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // BOLLINGER BANDS
    /// <include file='./info.xml' path='indicator/*' />
    ///
    public static IEnumerable<BollingerBandsResult> GetBollingerBands<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods = 20,
        double standardDeviations = 2)
        where TQuote : IQuote
    {
        // convert quotes
        List<BasicD> bdList = quotes.ConvertToBasic(CandlePart.Close);

        // check parameter arguments
        ValidateBollingerBands(quotes, lookbackPeriods, standardDeviations);

        // initialize
        List<BollingerBandsResult> results = new(bdList.Count);

        // roll through quotes
        for (int i = 0; i < bdList.Count; i++)
        {
            BasicD q = bdList[i];
            decimal close = (decimal)q.Value;
            int index = i + 1;

            BollingerBandsResult r = new()
            {
                Date = q.Date
            };

            if (index >= lookbackPeriods)
            {
                double[] periodClose = new double[lookbackPeriods];
                double sum = 0;
                int n = 0;

                for (int p = index - lookbackPeriods; p < index; p++)
                {
                    BasicD d = bdList[p];
                    periodClose[n] = d.Value;
                    sum += d.Value;
                    n++;
                }

                double periodAvg = sum / lookbackPeriods;
                double stdDev = Functions.StdDev(periodClose);

                r.Sma = (decimal)periodAvg;
                r.UpperBand = (decimal)(periodAvg + (standardDeviations * stdDev));
                r.LowerBand = (decimal)(periodAvg - (standardDeviations * stdDev));

                r.PercentB = (r.UpperBand == r.LowerBand) ? null
                    : (double)((close - r.LowerBand) / (r.UpperBand - r.LowerBand));

                r.ZScore = (stdDev == 0) ? null : (double)(close - r.Sma) / stdDev;
                r.Width = (periodAvg == 0) ? null : (double)(r.UpperBand - r.LowerBand) / periodAvg;
            }

            results.Add(r);
        }

        return results;
    }

    // remove recommended periods
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
    ///
    public static IEnumerable<BollingerBandsResult> RemoveWarmupPeriods(
        this IEnumerable<BollingerBandsResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.Width != null);

        return results.Remove(removePeriods);
    }

    // parameter validation
    private static void ValidateBollingerBands<TQuote>(
        IEnumerable<TQuote> quotes,
        int lookbackPeriods,
        double standardDeviations)
        where TQuote : IQuote
    {
        // check parameter arguments
        if (lookbackPeriods <= 1)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 1 for Bollinger Bands.");
        }

        if (standardDeviations <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(standardDeviations), standardDeviations,
                "Standard Deviations must be greater than 0 for Bollinger Bands.");
        }

        // check quotes
        int qtyHistory = quotes.Count();
        int minHistory = lookbackPeriods;
        if (config.UseBadQuotesException && qtyHistory < minHistory)
        {
            string message = "Insufficient quotes provided for Bollinger Bands.  " +
                string.Format(
                    EnglishCulture,
                    "You provided {0} periods of quotes when at least {1} are required.",
                    qtyHistory, minHistory);

            throw new BadQuotesException(nameof(quotes), message);
        }
    }
}
