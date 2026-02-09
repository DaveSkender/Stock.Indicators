namespace Skender.Stock.Indicators;

/// <summary>
/// Provides streaming calculation of the Hurst Exponent indicator.
/// </summary>
public class HurstHub
    : ChainHub<IReusable, HurstResult>, IHurst
{
    private readonly Queue<double> _buffer;

    internal HurstHub(
        IChainProvider<IReusable> provider,
        int lookbackPeriods) : base(provider)
    {
        Hurst.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;
        Name = $"HURST({lookbackPeriods})";
        _buffer = new Queue<double>(lookbackPeriods + 1);

        Reinitialize();
    }

    /// <inheritdoc/>
    public int LookbackPeriods { get; init; }
    /// <inheritdoc/>
    protected override (HurstResult result, int index)
        ToIndicator(IReusable item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);

        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // Add current value to buffer
        _buffer.Update(LookbackPeriods + 1, item.Value);

        double? h = null;

        // Need enough periods to calculate Hurst (lookbackPeriods + 1 values to get lookbackPeriods returns)
        if (_buffer.Count == LookbackPeriods + 1)
        {
            // Get evaluation batch - calculate returns from buffer values
            double[] values = new double[LookbackPeriods];
            double[] bufferArray = _buffer.ToArray();

            int x = 0;
            double l = bufferArray[0];

            // Skip first value (used as initial l) and calculate returns for the rest
            for (int p = 1; p < bufferArray.Length; p++)
            {
                double ps = bufferArray[p];

                // Return values
                values[x] = l != 0 ? (ps / l) - 1 : double.NaN;

                l = ps;
                x++;
            }

            // Calculate hurst exponent
            h = Hurst.CalcHurstWindow(values).NaN2Null();
        }

        // Candidate result
        HurstResult r = new(
            Timestamp: item.Timestamp,
            HurstExponent: h);

        return (r, i);
    }

    /// <summary>
    /// Restores the buffer state up to the specified timestamp.
    /// </summary>
    /// <inheritdoc/>
    protected override void RollbackState(DateTime timestamp)
    {
        // Clear buffer
        _buffer.Clear();

        if (timestamp <= DateTime.MinValue || ProviderCache.Count == 0)
        {
            return;
        }

        // Find the first index at or after timestamp
        int index = ProviderCache.IndexGte(timestamp);

        if (index <= 0)
        {
            // Rolling back before all data, keep cleared state
            return;
        }

        // We need to rebuild state up to the index before timestamp
        int targetIndex = index - 1;

        // Rebuild buffer from cache
        // We need at most the last (lookbackPeriods + 1) values
        int startIdx = Math.Max(0, targetIndex + 1 - (LookbackPeriods + 1));

        for (int p = startIdx; p <= targetIndex; p++)
        {
            IReusable item = ProviderCache[p];
            _buffer.Update(LookbackPeriods + 1, item.Value);
        }
    }
}

public static partial class Hurst
{
    /// <summary>
    /// Creates a Hurst Exponent streaming hub from a chain provider.
    /// </summary>
    /// <param name="chainProvider">The chain provider.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <returns>A Hurst hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the chain provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    public static HurstHub ToHurstHub(
        this IChainProvider<IReusable> chainProvider,
        int lookbackPeriods = 100)
             => new(chainProvider, lookbackPeriods);
}
