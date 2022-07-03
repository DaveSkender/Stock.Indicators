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
        ValidatePriceRelative(tpListEval, tpListBase, lookbackPeriods, smaPeriods);

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

    // parameter validation
    private static void ValidatePriceRelative(
        List<(DateTime, double)> quotesEval,
        List<(DateTime, double)> quotesBase,
        int? lookbackPeriods,
        int? smaPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods is not null and <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for Price Relative Strength.");
        }

        if (smaPeriods is not null and <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(smaPeriods), smaPeriods,
                "SMA periods must be greater than 0 for Price Relative Strength.");
        }

        // check quotes
        int qtyHistoryEval = quotesEval.Count;
        int qtyHistoryBase = quotesBase.Count;

        int? minHistory = lookbackPeriods;
        if (minHistory != null && qtyHistoryEval < minHistory)
        {
            string message = "Insufficient quotes provided for Price Relative Strength.  " +
                string.Format(
                    EnglishCulture,
                    "You provided {0} periods of quotes when at least {1} are required.",
                    qtyHistoryEval, minHistory);

            throw new InvalidQuotesException(nameof(quotesEval), message);
        }

        if (qtyHistoryBase != qtyHistoryEval)
        {
            throw new InvalidQuotesException(
                nameof(quotesBase),
                "Base quotes should have at least as many records as Eval quotes for PRS.");
        }
    }
}
