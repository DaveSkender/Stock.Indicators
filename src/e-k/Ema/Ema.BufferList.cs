namespace Skender.Stock.Indicators;

/// <summary>
/// Exponential Moving Average (EMA) from incremental reusable values.
/// </summary>
public class EmaList : IndicatorBufferListBase<EmaResult>, IExponentialIndicator
{
    private readonly Queue<double> _buffer;
    private double _bufferSum;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmaList"/> class.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    public EmaList(int lookbackPeriods)
    {
        IndicatorUtilities.ValidateLookbackPeriods(lookbackPeriods, "EMA");
        LookbackPeriods = lookbackPeriods;
        K = 2d / (lookbackPeriods + 1);

        _buffer = new Queue<double>(lookbackPeriods);
        _bufferSum = 0;
    }

    /// <inheritdoc/>
    public override int LookbackPeriods { get; }

    /// <inheritdoc/>
    public double K { get; private init; }

    /// <inheritdoc/>
    public override void Add(DateTime timestamp, double value)
    {
        // update buffer
        if (_buffer.Count == LookbackPeriods)
        {
            _bufferSum -= _buffer.Dequeue();
        }

        _buffer.Enqueue(value);
        _bufferSum += value;

        // add nulls for incalculable periods
        if (Count < LookbackPeriods - 1)
        {
            ((List<EmaResult>)this).Add(new EmaResult(timestamp));
            return;
        }

        double? lastEma = this[^1].Ema;

        // re/initialize as SMA
        if (lastEma is null)
        {
            ((List<EmaResult>)this).Add(new EmaResult(
                timestamp,
                _bufferSum / LookbackPeriods));
            return;
        }

        // calculate EMA normally
        ((List<EmaResult>)this).Add(new EmaResult(
            timestamp,
            Ema.Increment(K, lastEma.Value, value)));
    }
}
