namespace Skender.Stock.Indicators;

// AVERAGE DIRECTIONAL INDEX (SERIES)

public static partial class Indicator
{
    internal static List<AdxResult> CalcAdx(
        this List<QuoteD> qdList,
        int lookbackPeriods)
    {
        // check parameter arguments
        Adx.Validate(lookbackPeriods);

        // initialize
        int length = qdList.Count;
        List<AdxResult> results = new(length);

        if (length == 0)
        {
            return results;
        }

        // initialize input params
        QuoteD initQuote = qdList[0];

        AdxInput input = new(
            initQuote.High,
            initQuote.Low,
            initQuote.Close);

        // roll through quotes
        for (int i = 0; i < length; i++)
        {
            QuoteD q = qdList[i];

            AdxResult r = new(q.Date);
            results.Add(r);

            // skip first period
            if (i == 0)
            {
                continue;
            }

            // increment input params
            input.High = q.High;
            input.Low = q.Low;
            input.Close = q.Close;
            input.WindowAdx = i > (2 * lookbackPeriods) - 1
                ? results[i + 1 - lookbackPeriods].Adx.Null2NaN()
                : double.NaN;

            // get increment
            _ = Adx.Increment(i, lookbackPeriods, input, r);
        }

        return results;
    }
}
