namespace Skender.Stock.Indicators;

// FRACTAL CHAOS BANDS (SERIES)

public static partial class Indicator
{
    private static List<FcbResult> CalcFcb<TQuote>(
        this List<TQuote> quotesList,
        int windowSpan)
        where TQuote : IQuote
    {
        // check parameter arguments
        Fcb.Validate(windowSpan);

        // initialize
        int length = quotesList.Count;
        List<FcbResult> results = new(length);

        List<FractalResult> fractals = quotesList
            .CalcFractal(windowSpan, windowSpan, EndType.HighLow);

        decimal? upperLine = null;
        decimal? lowerLine = null;

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            FractalResult f = fractals[i];

            if (i >= 2 * windowSpan)
            {
                FractalResult fp = fractals[i - windowSpan];

                upperLine = fp.FractalBear ?? upperLine;
                lowerLine = fp.FractalBull ?? lowerLine;
            }

            results.Add(new(
                Timestamp: f.Timestamp,
                UpperBand: upperLine,
                LowerBand: lowerLine));
        }

        return results;
    }
}
