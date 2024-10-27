namespace Skender.Stock.Indicators;

// AVERAGE TRUE RANGE (SERIES)

public static partial class Atr
{
    public static IReadOnlyList<AtrResult> ToAtr<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        int lookbackPeriods = 14)
        where TQuote : IQuote => quotes
            .ToQuoteDList()
            .CalcAtr(lookbackPeriods);

    internal static List<AtrResult> CalcAtr(
        this IReadOnlyList<QuoteD> source,
        int lookbackPeriods)
    {
        // check parameter arguments
        Validate(lookbackPeriods);

        // initialize
        int length = source.Count;
        List<AtrResult> results = new(length);
        double prevAtr = double.NaN;
        double prevClose = double.NaN;
        double sumTr = 0;

        // skip first period
        if (length > 0)
        {
            QuoteD q = source[0];
            results.Add(new(Timestamp: q.Timestamp));
            prevClose = q.Close;
        }

        // roll through source values
        for (int i = 1; i < length; i++)
        {
            QuoteD q = source[i];

            double hmpc = Math.Abs(q.High - prevClose);
            double lmpc = Math.Abs(q.Low - prevClose);

            double tr = Math.Max(q.High - q.Low, Math.Max(hmpc, lmpc));

            double atr;
            double? atrp;

            if (i > lookbackPeriods)
            {
                // calculate ATR
                atr = ((prevAtr * (lookbackPeriods - 1)) + tr) / lookbackPeriods;
                atrp = q.Close == 0 ? null : atr / q.Close * 100;
                prevAtr = atr;
            }

            // TODO: update healing, without requiring specific indexing,
            // have had trouble gettng this one to work when evaluating previous ATR values
            else if (i == lookbackPeriods)
            {
                // initialize ATR
                sumTr += tr;
                atr = sumTr / lookbackPeriods;
                atrp = q.Close == 0 ? null : atr / q.Close * 100;
                prevAtr = atr;
            }

            // only used for initialization periods
            else
            {
                sumTr += tr;

                atr = double.NaN;
                atrp = null;
            }

            AtrResult r = new(
                Timestamp: q.Timestamp,
                Tr: tr.NaN2Null(),
                Atr: atr.NaN2Null(),
                Atrp: atrp);

            results.Add(r);

            prevClose = q.Close;
        }

        return results;
    }
}
