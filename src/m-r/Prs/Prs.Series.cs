namespace Skender.Stock.Indicators;

// PRICE RELATIVE STRENGTH (SERIES)

public static partial class Indicator
{
    internal static List<PrsResult> CalcPrs(
        List<(DateTime, double)> tpListEval,
        List<(DateTime, double)> tpListBase,
        int? lookbackPeriods = null)
    {
        // check parameter arguments
        Prs.Validate(tpListEval, tpListBase, lookbackPeriods);

        // initialize
        List<PrsResult> results = new(tpListEval.Count);

        // roll through quotes
        for (int i = 0; i < tpListEval.Count; i++)
        {
            (DateTime bDate, double bValue) = tpListBase[i];
            (DateTime eDate, double eValue) = tpListEval[i];

            if (eDate != bDate)
            {
                throw new InvalidQuotesException(nameof(tpListEval), eDate,
                    "Timestamp sequence does not match.  Price Relative requires matching dates in provided histories.");
            }

            PrsResult r = new() {
                Timestamp = eDate,
                Prs = (bValue == 0) ? null : (eValue / bValue).NaN2Null() // relative strength ratio
            };
            results.Add(r);

            if (lookbackPeriods != null && i > lookbackPeriods - 1)
            {
                (DateTime _, double boValue) = tpListBase[i - (int)lookbackPeriods];
                (DateTime _, double eoValue) = tpListEval[i - (int)lookbackPeriods];

                if (boValue != 0 && eoValue != 0)
                {
                    double? pctB = (bValue - boValue) / boValue;
                    double? pctE = (eValue - eoValue) / eoValue;

                    r.PrsPercent = (pctE - pctB).NaN2Null();
                }
            }
        }

        return results;
    }
}
