namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for calculating the T3 Moving Average indicator.
/// </summary>
public static partial class T3
{
    /// <summary>
    /// Creates a T3 hub from a chain provider.
    /// </summary>
    /// <typeparam name="T">The type of the reusable data.</typeparam>
    /// <param name="chainProvider">The chain provider.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <param name="volumeFactor">The volume factor for the calculation.</param>
    /// <returns>A T3 hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the chain provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods or volume factor are invalid.</exception>
    public static T3Hub<T> ToT3<T>(
        this IChainProvider<T> chainProvider,
        int lookbackPeriods = 5,
        double volumeFactor = 0.7)
        where T : IReusable
        => new(chainProvider, lookbackPeriods, volumeFactor);
}

/// <summary>
/// Represents a hub for T3 Moving Average calculations.
/// </summary>
/// <typeparam name="TIn">The type of the input data.</typeparam>
public class T3Hub<TIn>
    : ChainProvider<TIn, T3Result>, IT3
    where TIn : IReusable
{
    private readonly string hubName;
    private double lastE1 = double.NaN;
    private double lastE2 = double.NaN;
    private double lastE3 = double.NaN;
    private double lastE4 = double.NaN;
    private double lastE5 = double.NaN;
    private double lastE6 = double.NaN;

    internal T3Hub(
        IChainProvider<TIn> provider,
        int lookbackPeriods,
        double volumeFactor) : base(provider)
    {
        T3.Validate(lookbackPeriods, volumeFactor);
        LookbackPeriods = lookbackPeriods;
        VolumeFactor = volumeFactor;
        K = 2d / (lookbackPeriods + 1);

        double a = volumeFactor;
        C1 = -a * a * a;
        C2 = (3 * a * a) + (3 * a * a * a);
        C3 = (-6 * a * a) - (3 * a) - (3 * a * a * a);
        C4 = 1 + (3 * a) + (a * a * a) + (3 * a * a);

        hubName = $"T3({lookbackPeriods},{volumeFactor:F1})";

        Reinitialize();
    }

    /// <inheritdoc/>
    public int LookbackPeriods { get; init; }

    /// <inheritdoc/>
    public double VolumeFactor { get; init; }

    /// <inheritdoc/>
    public double K { get; private init; }

    /// <inheritdoc/>
    public double C1 { get; private init; }

    /// <inheritdoc/>
    public double C2 { get; private init; }

    /// <inheritdoc/>
    public double C3 { get; private init; }

    /// <inheritdoc/>
    public double C4 { get; private init; }

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <inheritdoc/>
    protected override (T3Result result, int index)
        ToIndicator(TIn item, int? indexHint)
    {
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // if out-of-order change (insertion/deletion before current index) occurred
        // invalidate state and backfill from previous cached EMA layers
        if (i > 0 && Cache.Count > i && Cache[i - 1].T3 is not null && (double.IsNaN(lastE1) || Cache[i - 1].E1 != lastE1))
        {
            lastE1 = Cache[i - 1].E1;
            lastE2 = Cache[i - 1].E2;
            lastE3 = Cache[i - 1].E3;
            lastE4 = Cache[i - 1].E4;
            lastE5 = Cache[i - 1].E5;
            lastE6 = Cache[i - 1].E6;
        }

        double t3 = CalculateIncrement(item.Value, i == 0);

        T3Result r = new(
            Timestamp: item.Timestamp,
            T3: t3.NaN2Null()) {
            E1 = lastE1,
            E2 = lastE2,
            E3 = lastE3,
            E4 = lastE4,
            E5 = lastE5,
            E6 = lastE6
        };

        return (r, i);
    }

    /// <inheritdoc/>
    protected override void RollbackState(DateTime timestamp)
    {
        int i = ProviderCache.IndexGte(timestamp);
        if (i > 0 && Cache.Count > i - 1)
        {
            T3Result prior = Cache[i - 1];
            lastE1 = prior.E1;
            lastE2 = prior.E2;
            lastE3 = prior.E3;
            lastE4 = prior.E4;
            lastE5 = prior.E5;
            lastE6 = prior.E6;
        }
        else
        {
            lastE1 = lastE2 = lastE3 = lastE4 = lastE5 = lastE6 = double.NaN;
        }
    }

    private double CalculateIncrement(double value, bool isFirst)
    {
        // re/seed values on first data point
        if (isFirst || double.IsNaN(lastE6))
        {
            lastE1 = lastE2 = lastE3 = lastE4 = lastE5 = lastE6 = value;
        }
        else
        {
            lastE1 = Ema.Increment(K, lastE1, value);
            lastE2 = Ema.Increment(K, lastE2, lastE1);
            lastE3 = Ema.Increment(K, lastE3, lastE2);
            lastE4 = Ema.Increment(K, lastE4, lastE3);
            lastE5 = Ema.Increment(K, lastE5, lastE4);
            lastE6 = Ema.Increment(K, lastE6, lastE5);
        }

        return (C1 * lastE6) + (C2 * lastE5) + (C3 * lastE4) + (C4 * lastE3);
    }
}
