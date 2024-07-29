namespace Skender.Stock.Indicators;

// EXPONENTIAL MOVING AVERAGE (INCREMENTING LIST)

public class EmaList<TQuote> : List<EmaResult>, IEma, IIncrementalPrice<TQuote>
    where TQuote : IQuote
{
    private readonly List<double> _buffer;

    public EmaList(int lookbackPeriods)
    {
        Ema.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;
        K = 2d / (lookbackPeriods + 1);

        _buffer = new(lookbackPeriods);
    }

    public int LookbackPeriods { get; init; }
    public double K { get; init; }

    public void Add(DateTime timestamp, double price)
    {
        // update buffer
        _buffer.Add(price);

        if (_buffer.Count > LookbackPeriods)
        {
            _buffer.RemoveAt(0);
        }

        // add nulls for incalculable periods
        if (Count < LookbackPeriods - 1)
        {
            base.Add(new EmaResult(timestamp));
            return;
        }

        // re/initialize
        if (this[^1].Ema is null)
        {
            double sum = 0;
            for (int i = 0; i < LookbackPeriods; i++)
            {
                sum += _buffer[i];
            }

            base.Add(new EmaResult(
                timestamp,
                sum / LookbackPeriods));

            return;
        }

        // calculate EMA normally
        base.Add(new EmaResult(
            timestamp,
            Ema.Increment(K, this[^1].Ema, price)));
    }

    public void Add(TQuote quote)
        => Add(quote.Timestamp, quote.Value);
}

/// <summary>
/// Exponential Moving Average (EMA) without date context (array-based).
/// </summary>
/// <inheritdoc cref="IIncrementalValue"/>
public class EmaArray : List<double?>, IEma, IIncrementalValue
{
    private readonly List<double> _buffer;

    public EmaArray(int lookbackPeriods)
    {
        Ema.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;
        K = 2d / (lookbackPeriods + 1);

        _buffer = new(lookbackPeriods);
    }

    public int LookbackPeriods { get; init; }
    public double K { get; init; }

    public void Add(double price)
    {
        // update buffer
        _buffer.Add(price);

        if (_buffer.Count > LookbackPeriods)
        {
            _buffer.RemoveAt(0);
        }

        // add nulls for incalculable periods
        if (Count < LookbackPeriods - 1)
        {
            Add(null);
            return;
        }

        // re/initialize
        if (this[^1] is null)
        {
            double sum = 0;
            for (int i = 0; i < LookbackPeriods; i++)
            {
                sum += _buffer[i];
            }

            base.Add(sum / LookbackPeriods);
            return;
        }

        // calculate EMA normally
        base.Add(Ema.Increment(K, this[^1], price));
    }
}
