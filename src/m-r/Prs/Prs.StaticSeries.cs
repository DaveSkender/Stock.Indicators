namespace Skender.Stock.Indicators;

// PRICE RELATIVE STRENGTH (SERIES)

public static partial class Prs
{
    public static IReadOnlyList<PrsResult> ToPrs<T>(
        this IReadOnlyList<T> sourceEval,
        IReadOnlyList<T> sourceBase,
        int? lookbackPeriods = null)
        where T : IReusable
    {
        // check parameter arguments
        ArgumentNullException.ThrowIfNull(sourceEval);
        ArgumentNullException.ThrowIfNull(sourceBase);
        Validate(sourceEval, sourceBase, lookbackPeriods);

        // initialize
        int length = sourceEval.Count;
        List<PrsResult> results = new(length);

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            T b = sourceBase[i];
            T e = sourceEval[i];

            if (e.Timestamp != b.Timestamp)
            {
                throw new InvalidQuotesException(
                    nameof(sourceEval), e.Timestamp,
                    "Timestamp sequence does not match.  "
                  + "Price Relative requires matching dates in provided histories.");
            }

            double? prsPercent = null;

            if (lookbackPeriods is not null && i > lookbackPeriods - 1)
            {
                T bo = sourceBase[i - (int)lookbackPeriods];
                T eo = sourceEval[i - (int)lookbackPeriods];

                if (bo.Value != 0 && eo.Value != 0)
                {
                    double? pctB = (b.Value - bo.Value) / bo.Value;
                    double? pctE = (e.Value - eo.Value) / eo.Value;

                    prsPercent = (pctE - pctB).NaN2Null();
                }
            }

            PrsResult r = new(
                Timestamp: e.Timestamp,

                Prs: b.Value == 0
                    ? null
                    : (e.Value / b.Value).NaN2Null(), // relative strength ratio

                PrsPercent: prsPercent);

            results.Add(r);
        }

        return results;
    }
}
