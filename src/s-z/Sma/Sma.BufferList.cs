namespace Skender.Stock.Indicators;

/// <summary>
/// Simple Moving Average (SMA) from incremental reusable values.
/// </summary>
public class SmaList : IndicatorBufferListBase<SmaResult>, ISinglePeriodIndicator
{
    private readonly Queue<double> _buffer;
    private double _bufferSum;

    /// <summary>
    /// Initializes a new instance of the <see cref="SmaList"/> class.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    public SmaList(int lookbackPeriods)
    {
        IndicatorUtilities.ValidateLookbackPeriods(lookbackPeriods, "SMA");
        LookbackPeriods = lookbackPeriods;

        _buffer = new Queue<double>(lookbackPeriods);
        _bufferSum = 0;
    }

    /// <inheritdoc/>
    public override int LookbackPeriods { get; }

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
            ((List<SmaResult>)this).Add(new SmaResult(timestamp, null));
            return;
        }

        // calculate SMA normally
        ((List<SmaResult>)this).Add(new SmaResult(
            timestamp,
            _bufferSum / LookbackPeriods));
    }
}