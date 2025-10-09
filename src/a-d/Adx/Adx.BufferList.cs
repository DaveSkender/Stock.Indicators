namespace Skender.Stock.Indicators;

/// <summary>
/// Average Directional Index (ADX) from incremental reusable values.
/// </summary>
public class AdxList : BufferList<AdxResult>, IAdx, IBufferList
{
    private readonly Queue<AdxBuffer> _buffer;

    /// <summary>
    /// Initializes a new instance of the <see cref="AdxList"/> class.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    public AdxList(int lookbackPeriods)
    {
        Adx.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;

        _buffer = new Queue<AdxBuffer>(lookbackPeriods);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AdxList"/> class with initial quotes.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <param name="quotes">Initial quotes to populate the list.</param>
    public AdxList(int lookbackPeriods, IReadOnlyList<IQuote> quotes)
        : this(lookbackPeriods)
        => Add(quotes);

    /// <summary>
    /// Gets the number of periods to look back for the calculation.
    /// </summary>
    public int LookbackPeriods { get; private init; }


    /// <inheritdoc />
    public void Add(IQuote quote)
    {
        ArgumentNullException.ThrowIfNull(quote);

        DateTime timestamp = quote.Timestamp;

        AdxBuffer curr = new(
            (double)quote.High,
            (double)quote.Low,
            (double)quote.Close);

        // skip first period
        if (Count == 0)
        {
            _buffer.Update(LookbackPeriods, curr);
            AddInternal(new AdxResult(timestamp));
            return;
        }

        // get last, then add current object using extension method
        AdxBuffer last = _buffer.Last();
        _buffer.Update(LookbackPeriods, curr);

        // calculate TR, PDM, and MDM
        double hmpc = Math.Abs(curr.High - last.Close);
        double lmpc = Math.Abs(curr.Low - last.Close);
        double hmph = curr.High - last.High;
        double plml = last.Low - curr.Low;

        curr.Tr = Math.Max(curr.High - curr.Low, Math.Max(hmpc, lmpc));

        curr.Pdm1 = hmph > plml ? Math.Max(hmph, 0) : 0;
        curr.Mdm1 = plml > hmph ? Math.Max(plml, 0) : 0;

        // skip incalculable
        if (Count < LookbackPeriods)
        {
            AddInternal(new AdxResult(timestamp));
            return;
        }

        // re/initialize smooth TR and DM
        if (Count >= LookbackPeriods && last.Trs == 0)
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
            AddInternal(new AdxResult(timestamp));
            return;
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
        if (Count < (2 * LookbackPeriods) - 1)
        {
            AddInternal(new AdxResult(timestamp,
                Pdi: pdi.NaN2Null(),
                Mdi: mdi.NaN2Null(),
                Dx: curr.Dx.NaN2Null()));

            return;
        }

        double adxr = double.NaN;

        // re/initialize ADX
        if (Count >= (2 * LookbackPeriods) - 1 && double.IsNaN(last.Adx))
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

        AddInternal(r);
    }

    /// <inheritdoc />
    public void Add(IReadOnlyList<IQuote> quotes)
    {
        ArgumentNullException.ThrowIfNull(quotes);

        for (int i = 0; i < quotes.Count; i++)
        {
            Add(quotes[i]);
        }
    }

    /// <inheritdoc />
    public override void Clear()
    {
        ClearInternal();
        _buffer.Clear();
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

public static partial class Adx
{
    /// <summary>
    /// Creates a buffer list for Average Directional Index (ADX) calculations.
    /// </summary>
    public static AdxList ToAdxList<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        int lookbackPeriods)
        where TQuote : IQuote
        => new(lookbackPeriods) { (IReadOnlyList<IQuote>)quotes };
}
