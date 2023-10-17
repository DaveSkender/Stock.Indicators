using System.Diagnostics.CodeAnalysis;

namespace Skender.Stock.Indicators;

// OBSOLETE IN v3
public static partial class Indicator
{
    // 3.0.0
    [ExcludeFromCodeCoverage]
    [Obsolete("Use a chained `results.GetSma(smaPeriods)` to generate a moving average.", false)]
    public static IEnumerable<AdlResult> GetAdl<TQuote>(
        this IEnumerable<TQuote> quotes,
        int smaPeriods)
        where TQuote : IQuote
    {
        List<AdlResult> results = quotes
            .ToQuoteD()
            .CalcAdl();

        List<SmaResult> sma = results
            .GetSma(smaPeriods)
            .ToList();

        int length = results.Count;

        for (int i = 0; i < length; i++)
        {
            results[i].AdlSma = sma[i].Sma;
        }

        return results;
    }
}
