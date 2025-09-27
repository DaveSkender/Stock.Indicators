namespace Skender.Stock.Indicators;

/// <summary>
/// Hull Moving Average (HMA) from incremental reusable values.
/// </summary>
public class HmaList : List<HmaResult>, IHma, IBufferQuote, IBufferReusable
{
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
    /// Initializes a new instance of the <see cref="HmaList"/> class.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    public HmaList(int lookbackPeriods)
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
    }

    /// <summary>
    /// Gets the number of periods to look back for the calculation.
    /// </summary>
    public int LookbackPeriods { get; init; }

    /// <summary>
    /// Adds a new value to the HMA list.
    /// </summary>
    /// <param name="timestamp">The timestamp of the value.</param>
    /// <param name="value">The value to add.</param>
    public void Add(DateTime timestamp, double value)
    {
        // update buffers for WMA calculations
        UpdateBuffers(value);

        double? hma = null;
        int shiftQty = LookbackPeriods - 1;

        // HMA calculation can only start after we have enough periods
        // to calculate both WMA values and then the synthetic buffer
        if (Count >= shiftQty)
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

        base.Add(new HmaResult(timestamp, hma));
    }

    /// <summary>
    /// Adds a new reusable value to the HMA list.
    /// </summary>
    /// <param name="value">The reusable value to add.</param>
    /// <exception cref="ArgumentNullException">Thrown when the value is null.</exception>
    public void Add(IReusable value)
    {
        ArgumentNullException.ThrowIfNull(value);
        Add(value.Timestamp, value.Value);
    }

    /// <summary>
    /// Adds a list of reusable values to the HMA list.
    /// </summary>
    /// <param name="values">The list of reusable values to add.</param>
    /// <exception cref="ArgumentNullException">Thrown when the values list is null.</exception>
    public void Add(IReadOnlyList<IReusable> values)
    {
        ArgumentNullException.ThrowIfNull(values);

        for (int i = 0; i < values.Count; i++)
        {
            Add(values[i].Timestamp, values[i].Value);
        }
    }

    /// <summary>
    /// Adds a new quote to the HMA list.
    /// </summary>
    /// <param name="quote">The quote to add.</param>
    /// <exception cref="ArgumentNullException">Thrown when the quote is null.</exception>
    public void Add(IQuote quote)
    {
        ArgumentNullException.ThrowIfNull(quote);
        Add(quote.Timestamp, quote.Value);
    }

    /// <summary>
    /// Adds a list of quotes to the HMA list.
    /// </summary>
    /// <param name="quotes">The list of quotes to add.</param>
    /// <exception cref="ArgumentNullException">Thrown when the quotes list is null.</exception>
    public void Add(IReadOnlyList<IQuote> quotes)
    {
        ArgumentNullException.ThrowIfNull(quotes);

        for (int i = 0; i < quotes.Count; i++)
        {
            Add(quotes[i].Timestamp, quotes[i].Value);
        }
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