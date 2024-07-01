namespace Skender.Stock.Indicators;

// ON-BALANCE VOLUME (SERIES)

public static partial class Indicator
{
    private static List<ObvResult> CalcObv(
        this List<QuoteD> qdList)
    {
        // initialize
        List<ObvResult> results = new(qdList.Count);

        double prevClose = double.NaN;
        double obv = 0;

        // roll through quotes
        for (int i = 0; i < qdList.Count; i++)
        {
            QuoteD q = qdList[i];

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

            ObvResult r = new() {
                Timestamp = q.Timestamp,
                Obv = obv
            };
            results.Add(r);

            prevClose = q.Close;
        }

        return results;
    }
}
