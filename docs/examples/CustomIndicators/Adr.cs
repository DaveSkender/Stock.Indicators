using System.Collections.ObjectModel;

using Skender.Stock.Indicators;

namespace Custom.Stock.Indicators;

// Custom results classes
// This inherits many of the extension methods, like .RemoveWarmupPeriods()
public sealed class DrResult : ResultBase, IReusableResult
{
    // date property is inherited here,
    // so you only need to add custom items
    public decimal Dr { get; set; }

    // to enable further chaining
    double? IReusableResult.Value => (double)Dr;
}

public sealed class AdrResult : ResultBase, IReusableResult
{
    public decimal Dr { get; set; }
    public double? Adr { get; set; }
    public double? Adrp { get; set; }

    double? IReusableResult.Value => Adrp;
}

public static partial class CustomIndicators
{
    // ADR calculation
    public static IEnumerable<AdrResult> GetAdr<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods)
        where TQuote : IQuote
    {
        // sort quotes and convert to collection or list
        Collection<TQuote> quotesList = quotes
            .ToSortedCollection();

        // initialize
        int length = quotes.Count();

        // average daily range
        List<DrResult> drResult = quotes
        .Select(x => new DrResult
        {
            Date = x.Date,
            Dr = x.High - x.Low
        }).ToList();

        List<AdrResult> results = drResult
            .GetSma(lookbackPeriods)
            .Select(x => new AdrResult
            {
                Date = x.Date,
                Adr = x.Sma
            }).ToList();

        for (int i = 0; i < length; i++)
        {
            results[i].Dr = drResult[i].Dr;
            results[i].Adrp = results[i].Adr / (double)quotesList[i].Close;
        }

        return results;
    }
}
