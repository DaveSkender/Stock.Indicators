namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for creating MFI stream hubs.
/// </summary>
public static partial class Mfi
{
    /// <summary>
    /// Converts the quote provider to an MFI hub.
    /// </summary>
    /// <typeparam name="TIn">The type of the input quote.</typeparam>
    /// <param name="quoteProvider">The quote provider.</param>
    /// <param name="lookbackPeriods">The number of lookback periods. Default is 14.</param>
    /// <returns>An MFI hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the quote provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    public static MfiHub<TIn> ToMfiHub<TIn>(
        this IQuoteProvider<TIn> quoteProvider,
        int lookbackPeriods = 14)
        where TIn : IQuote
    {
        ArgumentNullException.ThrowIfNull(quoteProvider);
        return new(quoteProvider, lookbackPeriods);
    }
}

/// <summary>
/// Streaming hub for the Money Flow Index (MFI) indicator.
/// </summary>
/// <typeparam name="TIn">The type of the input quote.</typeparam>
public class MfiHub<TIn> : ChainProvider<TIn, MfiResult>, IMfi
    where TIn : IQuote
{
    private readonly string hubName;

    /// <summary>
    /// Initializes a new instance of the <see cref="MfiHub{TIn}"/> class.
    /// </summary>
    /// <param name="provider">The quote provider.</param>
    /// <param name="lookbackPeriods">The number of lookback periods.</param>
    internal MfiHub(
        IQuoteProvider<TIn> provider,
        int lookbackPeriods)
        : base(provider)
    {
        Mfi.Validate(lookbackPeriods);

        LookbackPeriods = lookbackPeriods;
        hubName = $"MFI({lookbackPeriods})";

        Reinitialize();
    }

    /// <inheritdoc/>
    public int LookbackPeriods { get; init; }

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <inheritdoc/>
    protected override (MfiResult result, int index)
        ToIndicator(TIn item, int? indexHint)
    {
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        double? mfi = null;

        // Need at least lookbackPeriods + 1 quotes to calculate MFI
        if (i >= LookbackPeriods)
        {
            double sumPosMFs = 0;
            double sumNegMFs = 0;
            double? prevTruePrice = null;

            // Iterate through the lookback window
            for (int p = i - LookbackPeriods; p <= i; p++)
            {
                TIn q = ProviderCache[p];

                // Calculate true price
                double truePrice = ((double)q.High + (double)q.Low + (double)q.Close) / 3;

                // Calculate raw money flow
                double moneyFlow = truePrice * (double)q.Volume;

                // Determine direction (skip first item in window as it has no previous)
                if (prevTruePrice != null)
                {
                    if (truePrice > prevTruePrice)
                    {
                        sumPosMFs += moneyFlow;
                    }
                    else if (truePrice < prevTruePrice)
                    {
                        sumNegMFs += moneyFlow;
                    }
                    // If equal, don't add to either sum (direction = 0)
                }

                prevTruePrice = truePrice;
            }

            // Calculate MFI
            if (sumNegMFs != 0)
            {
                double mfRatio = sumPosMFs / sumNegMFs;
                mfi = 100 - (100 / (1 + mfRatio));
            }
            else
            {
                // Handle no negative case
                mfi = 100;
            }
        }

        // Create result
        MfiResult r = new(
            Timestamp: item.Timestamp,
            Mfi: mfi);

        return (r, i);
    }
}
