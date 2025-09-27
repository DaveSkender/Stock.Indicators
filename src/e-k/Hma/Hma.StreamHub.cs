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
        => new(chainProvider, lookbackPeriods);
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
    private readonly Queue<double> bufferN1;
    private readonly Queue<double> bufferN2;
    private readonly Queue<double> synthBuffer;
    private readonly double divisorN1;
    private readonly double divisorN2;
    private readonly double divisorSqrt;

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
        
        // initialize buffers for nested calculations
        bufferN1 = new Queue<double>(wmaN1Periods);
        bufferN2 = new Queue<double>(wmaN2Periods);
        synthBuffer = new Queue<double>(sqrtPeriods);
        
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
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // update buffers for WMA calculations
        UpdateBuffers(item.Value);

        double? hma = null;
        int shiftQty = LookbackPeriods - 1;

        // HMA calculation can only start after we have enough periods
        // to calculate both WMA values and then the synthetic buffer
        if (i >= shiftQty)
        {
            // calculate WMA(n/2) and WMA(n) for current period
            double? wmaN2 = CalculateWma(bufferN2, wmaN2Periods, divisorN2);
            double? wmaN1 = CalculateWma(bufferN1, wmaN1Periods, divisorN1);

            if (wmaN2.HasValue && wmaN1.HasValue)
            {
                // synthetic value: 2 * WMA(n/2) - WMA(n)
                double synthValue = (wmaN2.Value * 2d) - wmaN1.Value;
                
                // update synthetic buffer
                if (synthBuffer.Count == sqrtPeriods)
                {
                    synthBuffer.Dequeue();
                }
                synthBuffer.Enqueue(synthValue);
                
                // calculate final HMA = WMA(sqrt(n)) of synthetic values
                // Need enough synthetic values for the final WMA calculation
                if (synthBuffer.Count == sqrtPeriods)
                {
                    hma = CalculateWma(synthBuffer, sqrtPeriods, divisorSqrt);
                }
            }
        }

        // candidate result
        HmaResult r = new(
            Timestamp: item.Timestamp,
            Hma: hma);

        return (r, i);
    }

    private void UpdateBuffers(double value)
    {
        // update buffer for WMA(n)
        if (bufferN1.Count == wmaN1Periods)
        {
            bufferN1.Dequeue();
        }
        bufferN1.Enqueue(value);

        // update buffer for WMA(n/2)
        if (bufferN2.Count == wmaN2Periods)
        {
            bufferN2.Dequeue();
        }
        bufferN2.Enqueue(value);
    }

    private static double? CalculateWma(Queue<double> buffer, int periods, double divisor)
    {
        if (buffer.Count < periods)
            return null;

        double wma = 0;
        double[] values = buffer.ToArray();
        
        for (int j = 0; j < periods; j++)
        {
            wma += values[j] * (j + 1) / divisor;
        }
        
        return wma;
    }
}