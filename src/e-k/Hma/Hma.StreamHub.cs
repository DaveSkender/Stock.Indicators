namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for calculating the Hull Moving Average (HMA) indicator.
/// </summary>
public static partial class Hma
{
    /// <summary>
    /// Creates an HMA hub from a chain provider.
    /// </summary>
    /// <typeparam name="T">The type of the reusable data.</typeparam>
    /// <param name="chainProvider">The chain provider.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <returns>An HMA hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the chain provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    public static HmaHub<T> ToHma<T>(
        this IChainProvider<T> chainProvider,
        int lookbackPeriods)
        where T : IReusable
    {
        ArgumentNullException.ThrowIfNull(chainProvider);
        return new(chainProvider, lookbackPeriods);
    }
}

/// <summary>
/// Represents a hub for Hull Moving Average (HMA) calculations.
/// </summary>
/// <typeparam name="TIn">The type of the input data.</typeparam>
public class HmaHub<TIn>
    : ChainProvider<TIn, HmaResult>, IHma
    where TIn : IReusable
{
    private readonly string hubName;
    private readonly int wmaN1Periods;
    private readonly int wmaN2Periods;
    private readonly int sqrtPeriods;
    private readonly double divisorN1;
    private readonly double divisorN2;
    private readonly double divisorSqrt;
    private readonly int shiftQty;

    /// <summary>
    /// Initializes a new instance of the <see cref="HmaHub{TIn}"/> class.
    /// </summary>
    /// <param name="provider">The chain provider.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    internal HmaHub(
        IChainProvider<TIn> provider,
        int lookbackPeriods) : base(provider)
    {
        Hma.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;

        // initialize periods for nested WMA calculations
        wmaN1Periods = lookbackPeriods;
        wmaN2Periods = lookbackPeriods / 2;
        sqrtPeriods = (int)Math.Sqrt(lookbackPeriods);

        // calculate divisors for WMA
        divisorN1 = (double)wmaN1Periods * (wmaN1Periods + 1) / 2d;
        divisorN2 = (double)wmaN2Periods * (wmaN2Periods + 1) / 2d;
        divisorSqrt = (double)sqrtPeriods * (sqrtPeriods + 1) / 2d;

        shiftQty = lookbackPeriods - 1;

        hubName = $"HMA({lookbackPeriods})";
        Reinitialize();
    }

    /// <inheritdoc/>
    public int LookbackPeriods { get; init; }

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <inheritdoc/>
    protected override (HmaResult result, int index)
        ToIndicator(TIn item, int? indexHint)
    {
        int index = indexHint ?? ProviderCache.IndexOf(item, true);
        double? hma = CalculateHma(index);

        HmaResult result = new(
            Timestamp: item.Timestamp,
            Hma: hma);

        return (result, index);
    }

    private double? CalculateHma(int index)
    {
        int minIndex = shiftQty + sqrtPeriods - 1;
        if (index < minIndex)
        {
            return null;
        }

        double hma = 0d;
        int start = index - sqrtPeriods + 1;

        for (int offset = 0; offset < sqrtPeriods; offset++)
        {
            int synthIndex = start + offset;
            double? wmaN2 = CalculateWma(synthIndex, wmaN2Periods, divisorN2);
            double? wmaN1 = CalculateWma(synthIndex, wmaN1Periods, divisorN1);

            if (!wmaN2.HasValue || !wmaN1.HasValue)
            {
                return null;
            }

            double synthValue = (wmaN2.Value * 2d) - wmaN1.Value;
            hma += synthValue * (offset + 1) / divisorSqrt;
        }

        return hma;
    }

    private double? CalculateWma(int index, int periods, double divisor)
    {
        int start = index - periods + 1;
        if (start < 0)
        {
            return null;
        }

        double wma = 0d;

        for (int offset = 0; offset < periods; offset++)
        {
            double value = ProviderCache[start + offset].Value;
            if (double.IsNaN(value))
            {
                return null;
            }

            wma += value * (offset + 1) / divisor;
        }

        return wma;
    }
}
