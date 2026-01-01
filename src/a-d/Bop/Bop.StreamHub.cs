namespace Skender.Stock.Indicators;

/// <summary>
/// Streaming hub for Balance of Power (BOP).
/// </summary>
public class BopHub
    : ChainHub<IQuote, BopResult>, IBop
{
    internal BopHub(
        IQuoteProvider<IQuote> provider,
        int smoothPeriods) : base(provider)
    {
        Bop.Validate(smoothPeriods);
        SmoothPeriods = smoothPeriods;
        Name = $"BOP({smoothPeriods})";

        Reinitialize();
    }

    /// <inheritdoc/>
    public int SmoothPeriods { get; init; }
    /// <inheritdoc/>
    protected override (BopResult result, int index)
        ToIndicator(IQuote item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        double bop = double.NaN;

        if (i >= SmoothPeriods - 1)
        {
            double sum = 0;

            for (int p = i + 1 - SmoothPeriods; p <= i; p++)
            {
                IQuote quote = ProviderCache[p];

                // Calculate raw BOP value: (Close - Open) / (High - Low)
                // Match static series calculation order exactly
                double high = (double)quote.High;
                double low = (double)quote.Low;
                double close = (double)quote.Close;
                double open = (double)quote.Open;

                double range = high - low;
                if (range != 0)
                {
                    double rawBop = (close - open) / range;
                    sum += rawBop;
                }
                else
                {
                    sum += double.NaN;
                }
            }

            bop = double.IsNaN(sum) ? double.NaN : sum / SmoothPeriods;
        }

        // Candidate result
        BopResult r = new(
            Timestamp: item.Timestamp,
            Bop: bop.NaN2Null());

        return (r, i);
    }
}

public static partial class Bop
{
    /// <summary>
    /// Creates a Balance of Power hub from a chain provider.
    /// </summary>
    /// <param name="chainProvider">The chain provider.</param>
    /// <param name="smoothPeriods">The number of periods for smoothing. Default is 14.</param>
    /// <returns>A Balance of Power hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the chain provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the smooth periods are invalid.</exception>
    public static BopHub ToBopHub(
        this IQuoteProvider<IQuote> chainProvider,
        int smoothPeriods = 14)
        => new(chainProvider, smoothPeriods);
}
