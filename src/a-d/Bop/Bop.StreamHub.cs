namespace Skender.Stock.Indicators;

// BALANCE OF POWER (STREAM HUB)

/// <summary>
/// Provides methods for calculating the Balance of Power (BOP).
/// </summary>
public static partial class Bop
{
    /// <summary>
    /// Creates a Balance of Power hub from a chain provider.
    /// </summary>
    /// <typeparam name="T">The type of the quote data.</typeparam>
    /// <param name="chainProvider">The chain provider.</param>
    /// <param name="smoothPeriods">The number of periods for smoothing. Default is 14.</param>
    /// <returns>A Balance of Power hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the chain provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the smooth periods are invalid.</exception>
    public static BopHub<T> ToBopHub<T>(
        this IChainProvider<T> chainProvider,
        int smoothPeriods = 14)
        where T : IQuote
        => new(chainProvider, smoothPeriods);
}

/// <summary>
/// Represents a hub for Balance of Power calculations.
/// </summary>
/// <typeparam name="TIn">The type of the input data.</typeparam>
public class BopHub<TIn>
    : ChainProvider<TIn, BopResult>, IBop
    where TIn : IQuote
{
    private readonly string hubName;

    /// <summary>
    /// Initializes a new instance of the <see cref="BopHub{TIn}"/> class.
    /// </summary>
    /// <param name="provider">The chain provider.</param>
    /// <param name="smoothPeriods">The number of periods for smoothing.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the smooth periods are invalid.</exception>
    internal BopHub(
        IChainProvider<TIn> provider,
        int smoothPeriods) : base(provider)
    {
        Bop.Validate(smoothPeriods);
        SmoothPeriods = smoothPeriods;
        hubName = $"BOP({smoothPeriods})";

        Reinitialize();
    }

    /// <inheritdoc/>
    public int SmoothPeriods { get; init; }

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <inheritdoc/>
    protected override (BopResult result, int index)
        ToIndicator(TIn item, int? indexHint)
    {
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        double? bop = null;

        if (i >= SmoothPeriods - 1)
        {
            double sum = 0;

            for (int p = i + 1 - SmoothPeriods; p <= i; p++)
            {
                TIn quote = ProviderCache[p];

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

            bop = (sum / SmoothPeriods).NaN2Null();
        }

        // Candidate result
        BopResult r = new(
            Timestamp: item.Timestamp,
            Bop: bop);

        return (r, i);
    }
}
