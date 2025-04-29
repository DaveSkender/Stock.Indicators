namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for calculating the Rolling Pivot Points series.
/// </summary>
public static partial class RollingPivots
{
    /// <summary>
    /// Converts a list of quotes to a list of Rolling Pivot Points results.
    /// </summary>
    /// <typeparam name="TQuote">The type of the quote values.</typeparam>
    /// <param name="quotes">The list of quotes.</param>
    /// <param name="windowPeriods">The number of periods in the rolling window.</param>
    /// <param name="offsetPeriods">The number of periods to offset the window.</param>
    /// <param name="pointType">The type of pivot point calculation to use.</param>
    /// <returns>A list of Rolling Pivot Points results.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the quotes are null.</exception>
    [Series("ROLLING-PIVOTS", "Rolling Pivots", Category.PriceTrend, ChartType.Overlay)]
    public static IReadOnlyList<RollingPivotsResult> ToRollingPivots<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        [ParamNum<int>("Window Periods", 1, 250, 20)]
        int windowPeriods,
        [ParamNum<int>("Offset Periods", 0, 100, 0)]
        int offsetPeriods,
        [ParamEnum<PivotPointType>("Point Type", PivotPointType.Standard)]
        PivotPointType pointType = PivotPointType.Standard)
        where TQuote : IQuote
    {
        // check parameter arguments
        ArgumentNullException.ThrowIfNull(quotes);
        Validate(windowPeriods, offsetPeriods);

        // initialize
        int length = quotes.Count;
        List<RollingPivotsResult> results = new(length);

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            TQuote q = quotes[i];

            RollingPivotsResult r;

            if (i >= windowPeriods + offsetPeriods)
            {
                // window values
                int s = i - windowPeriods - offsetPeriods;
                TQuote hi = quotes[s];

                decimal windowHigh = hi.High;
                decimal windowLow = hi.Low;
                decimal windowClose = quotes[i - offsetPeriods - 1].Close;

                for (int p = s; p <= i - offsetPeriods - 1; p++)
                {
                    TQuote d = quotes[p];
                    windowHigh = d.High > windowHigh ? d.High : windowHigh;
                    windowLow = d.Low < windowLow ? d.Low : windowLow;
                }

                // pivot points
                WindowPoint wp = PivotPoints.GetPivotPoint(
                        pointType, q.Open, windowHigh, windowLow, windowClose);

                r = new() {

                    Timestamp = q.Timestamp,

                    // pivot point
                    PP = wp.PP,

                    // support
                    S1 = wp.S1,
                    S2 = wp.S2,
                    S3 = wp.S3,
                    S4 = wp.S4,

                    // resistance
                    R1 = wp.R1,
                    R2 = wp.R2,
                    R3 = wp.R3,
                    R4 = wp.R4
                };
            }
            else
            {
                r = new() { Timestamp = q.Timestamp };
            }

            results.Add(r);
        }

        return results;
    }
}
