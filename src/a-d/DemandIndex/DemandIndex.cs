namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // DEMAND INDEX (on CLOSE price)
    /// <include file='./info.xml' path='indicator/type[@name="Main"]/*' />
    ///
    public static IEnumerable<DemandIndexResult> GetDemandIndex<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods)
        where TQuote : IQuote
    {
        // convert quotes
        List<QuoteD> qdList = quotes.ConvertToList();

        // calculate
        return qdList.CalcDemandIndex(lookbackPeriods);
    }

    // DEMAND INDEX (on specified OHLCV part)
    /// <include file='./info.xml' path='indicator/type[@name="Custom"]/*' />
    ///
    public static IEnumerable<DemandIndexResult> GetDemandIndex<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods,
        CandlePart candlePart)
        where TQuote : IQuote
    {
        // convert quotes
        List<QuoteD> qdList = quotes.ConvertToList();

        // calculate
        return qdList.CalcDemandIndex(lookbackPeriods);
    }

    // remove recommended periods
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
    ///
    public static IEnumerable<DemandIndexResult> RemoveWarmupPeriods(
        this IEnumerable<DemandIndexResult> results)
    {
        int n = results
          .ToList()
          .FindIndex(x => x.DemandIndex != null) + 1;

        return results.Remove(n + 100);
    }

    // standard calculation
    private static List<DemandIndexResult> CalcDemandIndex(
        this List<QuoteD> qdList, int lookbackPeriods)
    {
        // check parameter arguments
        ValidateDemandIndex(lookbackPeriods);

        // initialize
        int length = qdList.Count;
        List<DemandIndexResult> results = new(length);

        // roll through quotes
        for (int i = 0; i < length; i++)
        {
            QuoteD h = qdList[i];
            int index = i + 1;

            DemandIndexResult result = new()
            {
                Date = h.Date
            };


            if (index > lookbackPeriods)
            {
                var src = h.Close;
                var prevSrc = i >= 1 ? qdList[i - 1].Close : 0;
                var vol = h.Volume;
                var pctChg = prevSrc != 0 ? (src - prevSrc) / Math.Abs(prevSrc) : 0;

                double rangeSma = 0;
                for (int p = index - lookbackPeriods; p < index; p++)
                {
                    var d = qdList[p];
                    var hh = Math.Max(d.High, qdList[p - 1].High);
                    var ll = Math.Min(d.Low, qdList[p - 1].Low);
                    rangeSma += hh - ll;
                }

                var va = rangeSma / lookbackPeriods;
                var k = va != 0 ? 3 * src / va : 0;
                var pFactor = pctChg * k;
                var bp = src > prevSrc ? vol : pFactor != 0 ? vol / pFactor : 0;
                var sp = src > prevSrc ? pFactor != 0 ? vol / pFactor : 0 : vol;

                result.DemandIndex = (decimal)(sp != 0 ? bp >= sp ? bp / sp : -bp / sp : 0);
            }
            else if (index == lookbackPeriods)
            {
                result.DemandIndex = 0;
            }

            results.Add(result);
        }

        return results;
    }

    // parameter validation
    private static void ValidateDemandIndex(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for DEMAND INDEX.");
        }
    }
}
