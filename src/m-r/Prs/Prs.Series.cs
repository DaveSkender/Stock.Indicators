namespace Skender.Stock.Indicators;

// PRICE RELATIVE STRENGTH (SERIES)

public static partial class Indicator
{
    internal static List<PrsResult> CalcPrs<T>(
        List<T> listEval,
        List<T> listBase,
        int? lookbackPeriods = null)
        where T : IReusable
    {
        // check parameter arguments
        Prs.Validate(listEval, listBase, lookbackPeriods);

        // initialize
        List<PrsResult> results = new(listEval.Count);

        // roll through quotes
        for (int i = 0; i < listEval.Count; i++)
        {
            T b = listBase[i];
            T e = listEval[i];

            if (e.Timestamp != b.Timestamp)
            {
                throw new InvalidQuotesException(nameof(listEval), e.Timestamp,
                    "Timestamp sequence does not match.  Price Relative requires matching dates in provided histories.");
            }

            PrsResult r = new() {
                Timestamp = e.Timestamp,
                Prs = (b.Value == 0) ? null : (e.Value / b.Value).NaN2Null() // relative strength ratio
            };
            results.Add(r);

            if (lookbackPeriods is not null && i > lookbackPeriods - 1)
            {
                T bo = listBase[i - (int)lookbackPeriods];
                T eo = listEval[i - (int)lookbackPeriods];

                if (bo.Value != 0 && eo.Value != 0)
                {
                    double? pctB = (b.Value - bo.Value) / bo.Value;
                    double? pctE = (e.Value - eo.Value) / eo.Value;

                    r.PrsPercent = (pctE - pctB).NaN2Null();
                }
            }
        }

        return results;
    }
}
