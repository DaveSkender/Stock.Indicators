namespace Skender.Stock.Indicators;

// ON-BALANCE VOLUME (SERIES)

public static partial class Indicator
{
    private static List<ObvResult> CalcObv(
        this List<QuoteD> source)
    {
        // initialize
        int length = source.Count;
        List<ObvResult> results = new(length);

        double prevClose = double.NaN;
        double obv = 0;

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            QuoteD q = source[i];

            if (double.IsNaN(prevClose) || q.Close == prevClose)
            {
                // no change to OBV
            }
            else if (q.Close > prevClose)
            {
                obv += q.Volume;
            }
            else if (q.Close < prevClose)
            {
                obv -= q.Volume;
            }

            results.Add(new ObvResult(
                Timestamp: q.Timestamp,
                Obv: obv));

            prevClose = q.Close;
        }

        return results;
    }
}
