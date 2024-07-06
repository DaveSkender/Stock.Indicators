namespace Skender.Stock.Indicators;

// WILLIAMS ALLIGATOR (STREAMING)

#region hub interface

public interface IAlligatorHub
{
    int JawPeriods { get; }
    int JawOffset { get; }
    int TeethPeriods { get; }
    int TeethOffset { get; }
    int LipsPeriods { get; }
    int LipsOffset { get; }
}
#endregion

public class Alligator<TIn>
    : ResultProvider<AlligatorResult>, IStreamHub<TIn, AlligatorResult>, IAlligatorHub
    where TIn : struct, IReusable
{
    private readonly StreamCache<AlligatorResult> _cache;
    private readonly StreamObserver<TIn, AlligatorResult> _observer;
    private readonly ChainProvider<TIn> _supplier;

    #region constructors

    public Alligator(
        ChainProvider<TIn> provider,
        int jawPeriods, int jawOffset,
        int teethPeriods, int teethOffset,
        int lipsPeriods, int lipsOffset)
        : this(provider, cache: new(),
            jawPeriods, jawOffset,
            teethPeriods, teethOffset,
            lipsPeriods, lipsOffset)
    { }

    private Alligator(
        ChainProvider<TIn> provider,
        StreamCache<AlligatorResult> cache,
        int jawPeriods, int jawOffset,
        int teethPeriods, int teethOffset,
        int lipsPeriods, int lipsOffset) : base(cache)
    {
        Alligator.Validate(
            jawPeriods, jawOffset,
            teethPeriods, teethOffset,
            lipsPeriods, lipsOffset);

        JawPeriods = jawPeriods;
        JawOffset = jawOffset;
        TeethPeriods = teethPeriods;
        TeethOffset = teethOffset;
        LipsPeriods = lipsPeriods;
        LipsOffset = lipsOffset;

        _cache = cache;
        _supplier = provider;
        _observer = new(this, this, provider);
    }
    #endregion

    public int JawPeriods { get; }
    public int JawOffset { get; }
    public int TeethPeriods { get; }
    public int TeethOffset { get; }
    public int LipsPeriods { get; }
    public int LipsOffset { get; }


    // METHODS

    public override string ToString()
        => $"ALLIGATOR({JawPeriods},{JawOffset},{TeethPeriods},{TeethOffset},{LipsPeriods},{LipsOffset})";

    public void Unsubscribe() => _observer.Unsubscribe();

    public void OnNextNew(TIn newItem)
    {
        double jaw = double.NaN;
        double lips = double.NaN;
        double teeth = double.NaN;

        int i = _supplier.Position(newItem);

        // calculate alligator's jaw, when in range
        if (i >= JawPeriods + JawOffset - 1)
        {
            AlligatorResult prev = ReadCache[i - 1];
            double prevJaw = prev.Jaw.Null2NaN();

            // first/reset value: calculate SMA
            if (double.IsNaN(prevJaw))
            {
                double sum = 0;
                for (int p = i - JawPeriods - JawOffset + 1; p <= i - JawOffset; p++)
                {
                    sum += _toValue(_supplier.ReadCache[p]);
                }

                jaw = sum / JawPeriods;
            }

            // remaining values: SMMA
            else
            {
                double newVal = _toValue(_supplier.ReadCache[i - JawOffset]);
                jaw = ((prevJaw * (JawPeriods - 1)) + newVal) / JawPeriods;
            }
        }

        // calculate alligator's teeth, when in range
        if (i >= TeethPeriods + TeethOffset - 1)
        {
            AlligatorResult prev = ReadCache[i - 1];

            double prevTooth = prev.Teeth.Null2NaN();

            // first/reset value: calculate SMA
            if (double.IsNaN(prevTooth))
            {
                double sum = 0;
                for (int p = i - TeethPeriods - TeethOffset + 1; p <= i - TeethOffset; p++)
                {
                    sum += _toValue(_supplier.ReadCache[p]);
                }

                teeth = sum / TeethPeriods;
            }

            // remaining values: SMMA
            else
            {
                double newVal = _toValue(_supplier.ReadCache[i - TeethOffset]);
                teeth = ((prevTooth * (TeethPeriods - 1)) + newVal) / TeethPeriods;
            }
        }

        // calculate alligator's lips, when in range
        if (i >= LipsPeriods + LipsOffset - 1)
        {
            AlligatorResult prev = ReadCache[i - 1];

            double prevLips = prev.Lips.Null2NaN();

            // first/reset value: calculate SMA
            if (double.IsNaN(prevLips))
            {
                // TODO: refactor - add offset to, and use Sma.Increment(...,offset)
                double sum = 0;
                for (int p = i - LipsPeriods - LipsOffset + 1; p <= i - LipsOffset; p++)
                {
                    sum += _toValue(_supplier.ReadCache[p]);
                }

                lips = sum / LipsPeriods;
            }

            // remaining values: SMMA
            else
            {
                double newVal = _toValue(_supplier.ReadCache[i - LipsOffset]);
                lips = ((prevLips * (LipsPeriods - 1)) + newVal) / LipsPeriods;
            }
        }

        // candidate result
        AlligatorResult r = new() {
            Timestamp = newItem.Timestamp,
            Jaw = jaw.NaN2Null(),
            Lips = lips.NaN2Null(),
            Teeth = teeth.NaN2Null()
        };

        // save to cache
        Act act = _cache.Modify(Act.AddNew, r);

        // send to observers
        NotifyObservers(act, r);
    }

    // convert provider IQuotes to HL2, if needed
    private readonly Func<TIn, double> _toValue
        = input => input is IQuote quote
        ? quote.ToReusable(CandlePart.HL2).Value
        : input.Value;
}
