namespace Skender.Stock.Indicators;

// FRACTAL CHAOS BANDS (SERIES)

public static partial class Indicator
{
    internal static List<FcbResult> CalcFcb<TQuote>(
        this List<TQuote> quotesList,
        int windowSpan)
        where TQuote : IQuote
    {
        // check parameter arguments
        Fcb.Validate(windowSpan);

        // initialize
        List<FractalResult> fractals = quotesList
            .CalcFractal(windowSpan, windowSpan, EndType.HighLow)
            .ToList();

        int length = fractals.Count;
        List<FcbResult> results = new(length);
        decimal? upperLine = null;
        decimal? lowerLine = null;

        // roll through quotes
        for (int i = 0; i < length; i++)
        {
            FractalResult f = fractals[i];

            FcbResult r = new(f.Date);
            results.Add(r);

            if (i >= 2 * windowSpan)
            {
                FractalResult fp = fractals[i - windowSpan];

                upperLine = fp.FractalBear ?? upperLine;
                lowerLine = fp.FractalBull ?? lowerLine;

                r.UpperBand = upperLine;
                r.LowerBand = lowerLine;
            }
        }

        return results;
    }
}
