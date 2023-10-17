namespace Skender.Stock.Indicators;

// PRICE RELATIVE STRENGTH (SERIES)

public static partial class Indicator
{
    internal static List<PrsResult> CalcPrs(
        List<(DateTime, double)> tpListEval,
        List<(DateTime, double)> tpListBase,
        int? lookbackPeriods = null,
        int? smaPeriods = null)
    {
        // check parameter arguments
        Prs.Validate(tpListEval, tpListBase, lookbackPeriods, smaPeriods);

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
                    "Date sequence does not match.  Price Relative requires matching dates in provided histories.");
            }

            PrsResult r = new(eDate)
            {
                Prs = (bValue == 0) ? null : (eValue / bValue).NaN2Null() // relative strength ratio
            };
            results.Add(r);

            if (lookbackPeriods != null && i + 1 > lookbackPeriods)
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

            // optional moving average of PRS
            if (smaPeriods != null && i + 1 >= smaPeriods)
            {
                double? sumRs = 0;
                for (int p = i + 1 - (int)smaPeriods; p <= i; p++)
                {
                    PrsResult d = results[p];
                    sumRs += d.Prs;
                }

                r.PrsSma = (sumRs / smaPeriods).NaN2Null();
            }
        }

        return results;
    }
}
