namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for calculating the Average Directional Index (ADX) indicator.
/// </summary>
public static partial class Adx
{
    /// <summary>
    /// Creates an ADX hub from a chain provider.
    /// </summary>
    /// <typeparam name="T">The type of the reusable data.</typeparam>
    /// <param name="chainProvider">The chain provider.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <returns>An ADX hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the chain provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    public static AdxHub<T> ToAdx<T>(
        this IChainProvider<T> chainProvider,
        int lookbackPeriods = 14)
        where T : IQuote
        => new(chainProvider, lookbackPeriods);
}

/// <summary>
/// Represents a hub for Average Directional Index (ADX) calculations.
/// </summary>
/// <typeparam name="TIn">The type of the input data.</typeparam>
public class AdxHub<TIn>
    : ChainProvider<TIn, AdxResult>, IAdx
    where TIn : IQuote
{
    private readonly string hubName;
    private readonly Queue<AdxBuffer> _buffer;

    /// <summary>
    /// Initializes a new instance of the <see cref="AdxHub{TIn}"/> class.
    /// </summary>
    /// <param name="provider">The chain provider.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    internal AdxHub(
        IChainProvider<TIn> provider,
        int lookbackPeriods) : base(provider)
    {
        Adx.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;
        hubName = $"ADX({lookbackPeriods})";

        _buffer = new Queue<AdxBuffer>(lookbackPeriods);

        Reinitialize();
    }

    /// <inheritdoc/>
    public int LookbackPeriods { get; init; }

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <inheritdoc/>
    protected override (AdxResult result, int index)
        ToIndicator(TIn item, int? indexHint)
    {
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // update buffer
        if (_buffer.Count == LookbackPeriods)
        {
            _buffer.Dequeue();
        }

        DateTime timestamp = item.Timestamp;

        AdxBuffer curr = new(
            (double)item.High,
            (double)item.Low,
            (double)item.Close);

        // skip first period
        if (i == 0)
        {
            _buffer.Enqueue(curr);
            AdxResult r0 = new(timestamp);
            return (r0, i);
        }

        // get last, then add current object
        AdxBuffer last = _buffer.Last();
        _buffer.Enqueue(curr);

        // calculate TR, PDM, and MDM
        double hmpc = Math.Abs(curr.High - last.Close);
        double lmpc = Math.Abs(curr.Low - last.Close);
        double hmph = curr.High - last.High;
        double plml = last.Low - curr.Low;

        curr.Tr = Math.Max(curr.High - curr.Low, Math.Max(hmpc, lmpc));

        curr.Pdm1 = hmph > plml ? Math.Max(hmph, 0) : 0;
        curr.Mdm1 = plml > hmph ? Math.Max(plml, 0) : 0;

        // skip incalculable
        if (i < LookbackPeriods)
        {
            AdxResult r1 = new(timestamp);
            return (r1, i);
        }

        // re/initialize smooth TR and DM
        if (i >= LookbackPeriods && last.Trs == 0)
        {
            foreach (AdxBuffer buffer in _buffer)
            {
                curr.Trs += buffer.Tr;
                curr.Pdm += buffer.Pdm1;
                curr.Mdm += buffer.Mdm1;
            }
        }

        // normal movement calculations
        else
        {
            curr.Trs = last.Trs - (last.Trs / LookbackPeriods) + curr.Tr;
            curr.Pdm = last.Pdm - (last.Pdm / LookbackPeriods) + curr.Pdm1;
            curr.Mdm = last.Mdm - (last.Mdm / LookbackPeriods) + curr.Mdm1;
        }

        // skip incalculable periods
        if (curr.Trs == 0)
        {
            AdxResult r2 = new(timestamp);
            return (r2, i);
        }

        // directional increments
        double pdi = 100 * curr.Pdm / curr.Trs;
        double mdi = 100 * curr.Mdm / curr.Trs;

        // calculate directional index (DX)
        curr.Dx = pdi - mdi == 0
            ? 0
            : pdi + mdi != 0
            ? 100 * Math.Abs(pdi - mdi) / (pdi + mdi)
            : double.NaN;

        // skip incalculable ADX periods
        if (i < (2 * LookbackPeriods) - 1)
        {
            AdxResult r3 = new(timestamp,
                Pdi: pdi.NaN2Null(),
                Mdi: mdi.NaN2Null(),
                Dx: curr.Dx.NaN2Null());

            return (r3, i);
        }

        double adxr = double.NaN;

        // re/initialize ADX
        if (i >= (2 * LookbackPeriods) - 1 && double.IsNaN(last.Adx))
        {
            double sumDx = 0;

            foreach (AdxBuffer buffer in _buffer)
            {
                sumDx += buffer.Dx;
            }

            curr.Adx = sumDx / LookbackPeriods;
        }

        // normal ADX calculation
        else
        {
            curr.Adx
                = ((last.Adx * (LookbackPeriods - 1)) + curr.Dx)
                / LookbackPeriods;

            AdxBuffer first = _buffer.Peek();
            adxr = (curr.Adx + first.Adx) / 2;
        }

        AdxResult r = new(
            Timestamp: timestamp,
            Pdi: pdi,
            Mdi: mdi,
            Dx: curr.Dx.NaN2Null(),
            Adx: curr.Adx.NaN2Null(),
            Adxr: adxr.NaN2Null());

        return (r, i);
    }

    /// <inheritdoc/>
    protected override void RollbackState(DateTime timestamp)
    {
        // Clear buffer state for rollback
        _buffer.Clear();
        
        // Find the index to rollback to
        int rollbackIndex = -1;
        for (int i = 0; i < Cache.Count; i++)
        {
            if (Cache[i].Timestamp >= timestamp)
            {
                rollbackIndex = i;
                break;
            }
        }

        // Rebuild buffer state from cache up to rollback point
        if (rollbackIndex >= 0)
        {
            int startIndex = Math.Max(0, rollbackIndex - LookbackPeriods);
            for (int i = startIndex; i < rollbackIndex; i++)
            {
                TIn providerItem = ProviderCache[i];
                AdxBuffer buffer = new(
                    (double)providerItem.High,
                    (double)providerItem.Low,
                    (double)providerItem.Close);

                if (i > startIndex)
                {
                    AdxBuffer lastBuffer = _buffer.Last();
                    
                    // Recalculate TR, PDM, MDM
                    double hmpc = Math.Abs(buffer.High - lastBuffer.Close);
                    double lmpc = Math.Abs(buffer.Low - lastBuffer.Close);
                    double hmph = buffer.High - lastBuffer.High;
                    double plml = lastBuffer.Low - buffer.Low;

                    buffer.Tr = Math.Max(buffer.High - buffer.Low, Math.Max(hmpc, lmpc));
                    buffer.Pdm1 = hmph > plml ? Math.Max(hmph, 0) : 0;
                    buffer.Mdm1 = plml > hmph ? Math.Max(plml, 0) : 0;

                    // Apply smoothing if we have enough data
                    if (i >= LookbackPeriods)
                    {
                        if (i == LookbackPeriods && lastBuffer.Trs == 0)
                        {
                            // Re-initialize smooth values
                            foreach (AdxBuffer b in _buffer)
                            {
                                buffer.Trs += b.Tr;
                                buffer.Pdm += b.Pdm1;
                                buffer.Mdm += b.Mdm1;
                            }
                            buffer.Trs += buffer.Tr;
                            buffer.Pdm += buffer.Pdm1;
                            buffer.Mdm += buffer.Mdm1;
                        }
                        else
                        {
                            buffer.Trs = lastBuffer.Trs - (lastBuffer.Trs / LookbackPeriods) + buffer.Tr;
                            buffer.Pdm = lastBuffer.Pdm - (lastBuffer.Pdm / LookbackPeriods) + buffer.Pdm1;
                            buffer.Mdm = lastBuffer.Mdm - (lastBuffer.Mdm / LookbackPeriods) + buffer.Mdm1;
                        }

                        // Calculate DX and ADX if possible
                        if (buffer.Trs != 0)
                        {
                            double pdi = 100 * buffer.Pdm / buffer.Trs;
                            double mdi = 100 * buffer.Mdm / buffer.Trs;

                            buffer.Dx = pdi - mdi == 0
                                ? 0
                                : pdi + mdi != 0
                                ? 100 * Math.Abs(pdi - mdi) / (pdi + mdi)
                                : double.NaN;

                            if (i >= (2 * LookbackPeriods) - 1)
                            {
                                if (i == (2 * LookbackPeriods) - 1 && double.IsNaN(lastBuffer.Adx))
                                {
                                    double sumDx = 0;
                                    foreach (AdxBuffer b in _buffer)
                                    {
                                        sumDx += b.Dx;
                                    }
                                    sumDx += buffer.Dx;
                                    buffer.Adx = sumDx / LookbackPeriods;
                                }
                                else if (!double.IsNaN(lastBuffer.Adx))
                                {
                                    buffer.Adx = ((lastBuffer.Adx * (LookbackPeriods - 1)) + buffer.Dx) / LookbackPeriods;
                                }
                            }
                        }
                    }
                }

                if (_buffer.Count == LookbackPeriods)
                {
                    _buffer.Dequeue();
                }
                _buffer.Enqueue(buffer);
            }
        }
    }

    internal class AdxBuffer(
        double high,
        double low,
        double close)
    {
        internal double High { get; init; } = high;
        internal double Low { get; init; } = low;
        internal double Close { get; init; } = close;

        internal double Tr { get; set; } = double.NaN;
        internal double Pdm1 { get; set; } = double.NaN;
        internal double Mdm1 { get; set; } = double.NaN;

        internal double Trs { get; set; }
        internal double Pdm { get; set; }
        internal double Mdm { get; set; }

        internal double Dx { get; set; } = double.NaN;
        internal double Adx { get; set; } = double.NaN;
    }
}