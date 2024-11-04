namespace Skender.Stock.Indicators;

/// <summary>
/// Provides extension methods for calculating the Chaikin Money Flow (CMF) on a series of quotes.
/// </summary>
public static partial class Cmf
{
    /// <summary>
    /// Calculates the Chaikin Money Flow (CMF) for a series of quotes.
    /// </summary>
    /// <typeparam name="TQuote">The type of the elements in the quotes list, which must implement <see cref="IQuote"/>.</typeparam>
    /// <param name="quotes">The source list of quotes.</param>
    /// <param name="lookbackPeriods">The number of periods to use for the lookback window. Default is 20.</param>
    /// <returns>A read-only list of <see cref="CmfResult"/> containing the CMF calculation results.</returns>
    public static IReadOnlyList<CmfResult> ToCmf<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        int lookbackPeriods = 20)
        where TQuote : IQuote => quotes
            .ToSortedList()
            .CalcCmf(lookbackPeriods);

    /// <summary>
    /// Calculates the Chaikin Money Flow (CMF) for a series of quotes.
    /// </summary>
    /// <typeparam name="TQuote">The type of the elements in the source list, which must implement <see cref="IQuote"/>.</typeparam>
    /// <param name="source">The source list of quotes.</param>
    /// <param name="lookbackPeriods">The number of periods to use for the lookback window.</param>
    /// <returns>A list of <see cref="CmfResult"/> containing the CMF calculation results.</returns>
    private static List<CmfResult> CalcCmf<TQuote>(
        this IReadOnlyList<TQuote> source,
        int lookbackPeriods)
        where TQuote : IQuote
    {
        // get volume array
        double[] volume
            = source.Select(v => (double)v.Volume).ToArray();

        // check parameter arguments
        Validate(lookbackPeriods);

        // initialize
        int length = volume.Length;
        List<CmfResult> results = new(length);
        IReadOnlyList<AdlResult> adlResults = source.ToAdl();

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            AdlResult adl = adlResults[i];
            double? cmf = null;

            if (i >= lookbackPeriods - 1)
            {
                double? sumMfv = 0;
                double? sumVol = 0;

                for (int p = i + 1 - lookbackPeriods; p <= i; p++)
                {
                    sumVol += volume[p];

                    AdlResult d = adlResults[p];
                    sumMfv += d.MoneyFlowVolume;
                }

                double? avgMfv = sumMfv / lookbackPeriods;
                double? avgVol = sumVol / lookbackPeriods;

                if (avgVol != 0)
                {
                    cmf = avgMfv / avgVol;
                }
            }

            results.Add(new(
                Timestamp: adl.Timestamp,
                MoneyFlowMultiplier: adl.MoneyFlowMultiplier,
                MoneyFlowVolume: adl.MoneyFlowVolume,
                Cmf: cmf));
        }

        return results;
    }
}
