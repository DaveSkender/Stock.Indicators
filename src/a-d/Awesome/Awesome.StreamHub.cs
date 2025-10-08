namespace Skender.Stock.Indicators;

// AWESOME OSCILLATOR (STREAM HUB)

/// <summary>
/// Provides methods for calculating the Awesome Oscillator.
/// </summary>
public static partial class Awesome
{
    /// <summary>
    /// Creates an Awesome Oscillator hub from a chain provider.
    /// </summary>
    /// <typeparam name="T">The type of the reusable data.</typeparam>
    /// <param name="chainProvider">The chain provider.</param>
    /// <param name="fastPeriods">The number of periods for the fast moving average. Default is 5.</param>
    /// <param name="slowPeriods">The number of periods for the slow moving average. Default is 34.</param>
    /// <returns>An Awesome Oscillator hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the chain provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the periods are invalid.</exception>
    public static AwesomeHub<T> ToAwesome<T>(
        this IChainProvider<T> chainProvider,
        int fastPeriods = 5,
        int slowPeriods = 34)
        where T : IReusable
        => new(chainProvider, fastPeriods, slowPeriods);
}

/// <summary>
/// Represents a hub for Awesome Oscillator calculations.
/// </summary>
/// <typeparam name="TIn">The type of the input data.</typeparam>
public class AwesomeHub<TIn>
    : ChainProvider<TIn, AwesomeResult>, IAwesome
    where TIn : IReusable
{
    private readonly string hubName;

    /// <summary>
    /// Initializes a new instance of the <see cref="AwesomeHub{TIn}"/> class.
    /// </summary>
    /// <param name="provider">The chain provider.</param>
    /// <param name="fastPeriods">The number of periods for the fast moving average.</param>
    /// <param name="slowPeriods">The number of periods for the slow moving average.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the periods are invalid.</exception>
    internal AwesomeHub(
        IChainProvider<TIn> provider,
        int fastPeriods,
        int slowPeriods) : base(provider)
    {
        Awesome.Validate(fastPeriods, slowPeriods);
        FastPeriods = fastPeriods;
        SlowPeriods = slowPeriods;
        hubName = $"AWESOME({fastPeriods},{slowPeriods})";

        Reinitialize();
    }

    /// <inheritdoc/>
    public int FastPeriods { get; init; }

    /// <inheritdoc/>
    public int SlowPeriods { get; init; }

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <inheritdoc/>
    protected override (AwesomeResult result, int index)
        ToIndicator(TIn item, int? indexHint)
    {
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        double? oscillator = null;
        double? normalized = null;

        if (i >= SlowPeriods - 1)
        {
            double sumSlow = 0;
            double sumFast = 0;

            for (int p = i + 1 - SlowPeriods; p <= i; p++)
            {
                // Use HL2 when available (IQuote), matching static series behavior
                double value = ProviderCache[p].Hl2OrValue();
                sumSlow += value;

                if (p >= i + 1 - FastPeriods)
                {
                    sumFast += value;
                }
            }

            double avgFast = sumFast / FastPeriods;
            double avgSlow = sumSlow / SlowPeriods;
            oscillator = (avgFast - avgSlow).NaN2Null();

            // Use HL2 for current value too
            double currentValue = item.Hl2OrValue();
            normalized = currentValue != 0 ? 100 * oscillator / currentValue : null;
        }

        // Candidate result
        AwesomeResult r = new(
            Timestamp: item.Timestamp,
            Oscillator: oscillator,
            Normalized: normalized);

        return (r, i);
    }
}
