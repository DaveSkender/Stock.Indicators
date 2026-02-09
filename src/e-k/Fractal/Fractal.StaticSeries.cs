namespace Skender.Stock.Indicators;

/// <summary>
/// Williams Fractal indicator.
/// </summary>
public static partial class Fractal
{
    /// <summary>
    /// Converts a list of quotes to Fractal results using the same span for both left and right wings.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="windowSpan">The number of periods to look back and forward for the calculation. Default is 2.</param>
    /// <param name="endType">The type of price to use for the calculation. Default is HighLow.</param>
    /// <returns>A list of Fractal results.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the quotes list is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the window span is invalid.</exception>
    public static IReadOnlyList<FractalResult> ToFractal(
        this IReadOnlyList<IQuote> quotes,
        int windowSpan = 2,
        EndType endType = EndType.HighLow)
        => quotes
            .ToFractal(windowSpan, windowSpan, endType);

    /// <summary>
    /// Converts a list of quotes to Fractal results using different spans for left and right wings.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="leftSpan">The number of periods to look back for the calculation.</param>
    /// <param name="rightSpan">The number of periods to look forward for the calculation.</param>
    /// <param name="endType">The type of price to use for the calculation. Default is HighLow.</param>
    /// <returns>A list of Fractal results.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the quotes list is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the left or right span is invalid.</exception>
    public static IReadOnlyList<FractalResult> ToFractal(
        this IReadOnlyList<IQuote> quotes,
        int leftSpan,
        int rightSpan,
        EndType endType = EndType.HighLow)
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
            IQuote q = quotes[i];
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
                    IQuote wing = quotes[p];

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
