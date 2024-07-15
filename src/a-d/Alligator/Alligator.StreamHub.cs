namespace Skender.Stock.Indicators;

// WILLIAMS ALLIGATOR (STREAM HUB)

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

public class AlligatorHub<TIn> : ReusableObserver<TIn, AlligatorResult>,
    IResultHub<TIn, AlligatorResult>, IAlligatorHub
    where TIn : IReusable
{
    #region constructors

    public AlligatorHub(
        IChainProvider<TIn> provider,
        int jawPeriods, int jawOffset,
        int teethPeriods, int teethOffset,
        int lipsPeriods, int lipsOffset)
        : base(provider)
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

        Reinitialize();
    }
    #endregion

    public int JawPeriods { get; }
    public int JawOffset { get; }
    public int TeethPeriods { get; }
    public int TeethOffset { get; }
    public int LipsPeriods { get; }
    public int LipsOffset { get; }

    // METHODS

    internal override void Add(Act act, TIn newIn, int? index)
    {
        if (newIn is null)
        {
            throw new ArgumentNullException(nameof(newIn));
        }

        double jaw = double.NaN;
        double lips = double.NaN;
        double teeth = double.NaN;

        int i = index ?? Supplier.GetIndex(newIn, false);

        // calculate alligator's jaw, when in range
        if (i >= JawPeriods + JawOffset - 1)
        {
            AlligatorResult prev = Cache[i - 1];
            double prevJaw = prev.Jaw.Null2NaN();

            // first/reset value: calculate SMA
            if (double.IsNaN(prevJaw))
            {
                double sum = 0;
                for (int p = i - JawPeriods - JawOffset + 1; p <= i - JawOffset; p++)
                {
                    sum += Supplier.Results[p].HL2ifQuote();
                }

                jaw = sum / JawPeriods;
            }

            // remaining values: SMMA
            else
            {
                double newVal = Supplier.Results[i - JawOffset].HL2ifQuote();
                jaw = ((prevJaw * (JawPeriods - 1)) + newVal) / JawPeriods;
            }
        }

        // calculate alligator's teeth, when in range
        if (i >= TeethPeriods + TeethOffset - 1)
        {
            AlligatorResult prev = Cache[i - 1];

            double prevTooth = prev.Teeth.Null2NaN();

            // first/reset value: calculate SMA
            if (double.IsNaN(prevTooth))
            {
                double sum = 0;
                for (int p = i - TeethPeriods - TeethOffset + 1; p <= i - TeethOffset; p++)
                {
                    sum += Supplier.Results[p].HL2ifQuote();
                }

                teeth = sum / TeethPeriods;
            }

            // remaining values: SMMA
            else
            {
                double newVal = Supplier.Results[i - TeethOffset].HL2ifQuote();
                teeth = ((prevTooth * (TeethPeriods - 1)) + newVal) / TeethPeriods;
            }
        }

        // calculate alligator's lips, when in range
        if (i >= LipsPeriods + LipsOffset - 1)
        {
            AlligatorResult prev = Cache[i - 1];

            double prevLips = prev.Lips.Null2NaN();

            // first/reset value: calculate SMA
            if (double.IsNaN(prevLips))
            {
                // TODO: refactor - add offset to, and use Sma.Increment(...,offset)
                double sum = 0;
                for (int p = i - LipsPeriods - LipsOffset + 1; p <= i - LipsOffset; p++)
                {
                    sum += Supplier.Results[p].HL2ifQuote();
                }

                lips = sum / LipsPeriods;
            }

            // remaining values: SMMA
            else
            {
                double newVal = Supplier.Results[i - LipsOffset].HL2ifQuote();
                lips = ((prevLips * (LipsPeriods - 1)) + newVal) / LipsPeriods;
            }
        }

        // candidate result
        AlligatorResult r = new(
            newIn.Timestamp, jaw.NaN2Null(), teeth.NaN2Null(), lips.NaN2Null());

        // save and send
        Motify(act, r, i);
    }

    public override string ToString()
        => $"ALLIGATOR({JawPeriods},{JawOffset},{TeethPeriods},{TeethOffset},{LipsPeriods},{LipsOffset})";
}
