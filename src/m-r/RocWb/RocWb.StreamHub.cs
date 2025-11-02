namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for calculating the Rate of Change with Bands (RocWb) indicator.
/// </summary>
public class RocWbHub
    : ChainProvider<IReusable, RocWbResult>, IRocWb
{
    private readonly string hubName;
    private readonly double k;
    private double prevEma = double.NaN;
    private readonly Queue<double> rocSqBuffer;
    private readonly Queue<double> rocEmaInitBuffer;

    /// <summary>
    /// Initializes a new instance of the <see cref="RocWbHub"/> class.
    /// </summary>
    /// <param name="provider">The chain provider.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window for ROC.</param>
    /// <param name="emaPeriods">Quantity of periods for EMA smoothing.</param>
    /// <param name="stdDevPeriods">Quantity of periods for standard deviation bands.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the parameters are invalid.</exception>
    internal RocWbHub(
        IChainProvider<IReusable> provider,
        int lookbackPeriods,
        int emaPeriods,
        int stdDevPeriods) : base(provider)
    {
        RocWb.Validate(lookbackPeriods, emaPeriods, stdDevPeriods);
        LookbackPeriods = lookbackPeriods;
        EmaPeriods = emaPeriods;
        StdDevPeriods = stdDevPeriods;
        k = 2d / (emaPeriods + 1);
        rocSqBuffer = new Queue<double>(stdDevPeriods);
        rocEmaInitBuffer = new Queue<double>(emaPeriods);
        hubName = $"ROCWB({lookbackPeriods},{emaPeriods},{stdDevPeriods})";

        Reinitialize();
    }

    /// <inheritdoc/>
    public int LookbackPeriods { get; init; }

    /// <inheritdoc/>
    public int EmaPeriods { get; init; }

    /// <inheritdoc/>
    public int StdDevPeriods { get; init; }

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <inheritdoc/>
    protected override (RocWbResult result, int index)
        ToIndicator(IReusable item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);

        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // Calculate ROC
        double roc;
        if (i >= LookbackPeriods)
        {
            IReusable back = ProviderCache[i - LookbackPeriods];
            roc = Math.Abs(back.Value) < double.Epsilon
                ? double.NaN
                : 100d * (item.Value - back.Value) / back.Value;
        }
        else
        {
            roc = double.NaN;
        }

        // Update the ROC squared buffer for std dev calculation
        double rocSq = roc * roc;
        if (!double.IsNaN(roc))
        {
            if (rocSqBuffer.Count >= StdDevPeriods)
            {
                rocSqBuffer.Dequeue();
            }

            rocSqBuffer.Enqueue(rocSq);
        }

        // Calculate EMA of ROC
        double rocEma;
        if (double.IsNaN(prevEma))
        {
            // Accumulate ROC values for initial EMA calculation
            if (!double.IsNaN(roc))
            {
                if (rocEmaInitBuffer.Count >= EmaPeriods)
                {
                    rocEmaInitBuffer.Dequeue();
                }

                rocEmaInitBuffer.Enqueue(roc);
            }

            // Initialize EMA with SMA when we have enough values
            if (rocEmaInitBuffer.Count >= EmaPeriods)
            {
                double sum = 0;
                foreach (double rocVal in rocEmaInitBuffer)
                {
                    sum += rocVal;
                }

                rocEma = sum / EmaPeriods;
            }
            else
            {
                rocEma = double.NaN;
            }
        }
        else
        {
            // Normal EMA calculation
            rocEma = Ema.Increment(k, prevEma, roc);
        }

        prevEma = rocEma;

        // Calculate RMS deviation bands
        double? rocDev = null;
        if (rocSqBuffer.Count >= StdDevPeriods)
        {
            double sum = 0;
            foreach (double sq in rocSqBuffer)
            {
                sum += sq;
            }

            rocDev = Math.Sqrt(sum / StdDevPeriods).NaN2Null();
        }

        // candidate result
        RocWbResult r = new(
            Timestamp: item.Timestamp,
            Roc: roc.NaN2Null(),
            RocEma: rocEma.NaN2Null(),
            UpperBand: rocDev,
            LowerBand: rocDev.HasValue ? -rocDev.Value : null);

        return (r, i);
    }

    /// <inheritdoc/>
    protected override void RollbackState(DateTime timestamp)
    {
        // Clear state
        prevEma = double.NaN;
        rocSqBuffer.Clear();
        rocEmaInitBuffer.Clear();

        // Rebuild from ProviderCache
        int index = ProviderCache.IndexGte(timestamp);
        if (index <= 0) return;

        int targetIndex = index - 1;

        // Replay state up to target index
        for (int p = 0; p <= targetIndex; p++)
        {
            IReusable current = ProviderCache[p];

            // Calculate ROC
            double roc;
            if (p >= LookbackPeriods)
            {
                IReusable back = ProviderCache[p - LookbackPeriods];
                roc = Math.Abs(back.Value) < double.Epsilon
                    ? double.NaN
                    : 100d * (current.Value - back.Value) / back.Value;
            }
            else
            {
                roc = double.NaN;
            }

            // Update ROC squared buffer
            double rocSq = roc * roc;
            if (!double.IsNaN(roc))
            {
                if (rocSqBuffer.Count >= StdDevPeriods)
                {
                    rocSqBuffer.Dequeue();
                }

                rocSqBuffer.Enqueue(rocSq);
            }

            // Calculate EMA of ROC
            double rocEma;
            if (double.IsNaN(prevEma))
            {
                // Accumulate ROC values for initial EMA calculation
                if (!double.IsNaN(roc))
                {
                    if (rocEmaInitBuffer.Count >= EmaPeriods)
                    {
                        rocEmaInitBuffer.Dequeue();
                    }

                    rocEmaInitBuffer.Enqueue(roc);
                }

                // Initialize EMA with SMA when we have enough values
                if (rocEmaInitBuffer.Count >= EmaPeriods)
                {
                    double sum = 0;
                    foreach (double rocVal in rocEmaInitBuffer)
                    {
                        sum += rocVal;
                    }

                    rocEma = sum / EmaPeriods;
                }
                else
                {
                    rocEma = double.NaN;
                }
            }
            else
            {
                // Normal EMA calculation
                rocEma = Ema.Increment(k, prevEma, roc);
            }

            prevEma = rocEma;
        }
    }
}


public static partial class RocWb
{
    /// <summary>
    /// Creates a RocWb streaming hub from a chain provider.
    /// </summary>
    /// <param name="chainProvider">The chain provider.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window for ROC.</param>
    /// <param name="emaPeriods">Quantity of periods for EMA smoothing.</param>
    /// <param name="stdDevPeriods">Quantity of periods for standard deviation bands.</param>
    /// <returns>A RocWb hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the chain provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the parameters are invalid.</exception>
    public static RocWbHub ToRocWbHub(
        this IChainProvider<IReusable> chainProvider,
        int lookbackPeriods = 20,
        int emaPeriods = 5,
        int stdDevPeriods = 5)
        => new(chainProvider, lookbackPeriods, emaPeriods, stdDevPeriods);

    /// <summary>
    /// Creates a RocWb hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window for ROC.</param>
    /// <param name="emaPeriods">Quantity of periods for EMA smoothing.</param>
    /// <param name="stdDevPeriods">Quantity of periods for standard deviation bands.</param>
    /// <returns>An instance of <see cref="RocWbHub"/>.</returns>
    public static RocWbHub ToRocWbHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 20,
        int emaPeriods = 5,
        int stdDevPeriods = 5)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToRocWbHub(lookbackPeriods, emaPeriods, stdDevPeriods);
    }

}
