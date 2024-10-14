namespace Skender.Stock.Indicators;

// WILLIAMS FRACTAL (SERIES)

public static partial class Fractal
{
    public static IReadOnlyList<FractalResult> ToFractal<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        int windowSpan = 2,
        EndType endType = EndType.HighLow)
        where TQuote : IQuote => quotes
            .ToFractal(windowSpan, windowSpan, endType);

    public static IReadOnlyList<FractalResult> ToFractal<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        int leftSpan,
        int rightSpan,
        EndType endType = EndType.HighLow)
        where TQuote : IQuote
    {
        // check parameter arguments
        ArgumentNullException.ThrowIfNull(quotes);
        Validate(Math.Min(leftSpan, rightSpan));

        // initialize
        int length = quotes.Count;
        List<FractalResult> results = new(length);

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            TQuote q = quotes[i];
            decimal? fractalBear = null;
            decimal? fractalBull = null;

            if (i + 1 > leftSpan && i + 1 <= length - rightSpan)
            {
                bool isHigh = true;
                bool isLow = true;

                decimal evalHigh = endType == EndType.Close ?
                   q.Close : q.High;

                decimal evalLow = endType == EndType.Close ?
                    q.Close : q.Low;

                // compare today with wings
                for (int p = i - leftSpan; p <= i + rightSpan; p++)
                {
                    // skip center eval period
                    if (p == i)
                    {
                        continue;
                    }

                    // evaluate wing periods
                    TQuote wing = quotes[p];

                    decimal wingHigh = endType == EndType.Close ?
                        wing.Close : wing.High;

                    decimal wingLow = endType == EndType.Close ?
                        wing.Close : wing.Low;

                    if (evalHigh <= wingHigh)
                    {
                        isHigh = false;
                    }

                    if (evalLow >= wingLow)
                    {
                        isLow = false;
                    }
                }

                // bearish signal
                if (isHigh)
                {
                    fractalBear = evalHigh;
                }

                // bullish signal
                if (isLow)
                {
                    fractalBull = evalLow;
                }
            }

            results.Add(new(
                Timestamp: q.Timestamp,
                FractalBear: fractalBear,
                FractalBull: fractalBull));
        }

        return results;
    }
}
