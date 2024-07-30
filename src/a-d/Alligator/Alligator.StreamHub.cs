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

    internal AlligatorHub(
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

    public int JawPeriods { get; init; }
    public int JawOffset { get; init; }
    public int TeethPeriods { get; init; }
    public int TeethOffset { get; init; }
    public int LipsPeriods { get; init; }
    public int LipsOffset { get; init; }

    // METHODS

    internal override void Add(Act act, TIn newIn, int? index)
    {
        double jaw = double.NaN;
        double lips = double.NaN;
        double teeth = double.NaN;

        int i = index ?? Provider.GetIndex(newIn, false);

        // calculate alligator's jaw, when in range
        if (i >= JawPeriods + JawOffset - 1)
        {
            // first/reset value: calculate SMA
            if (Cache[i - 1].Jaw is null)
            {
                double sum = 0;
                for (int p = i - JawPeriods - JawOffset + 1; p <= i - JawOffset; p++)
                {
                    sum += Provider.Results[p].Hl2OrValue();
                }

                jaw = sum / JawPeriods;
            }

            // remaining values: SMMA
            else
            {
                double prevJaw = Cache[i - 1].Jaw.Null2NaN();
                double newVal = Provider.Results[i - JawOffset].Hl2OrValue();

                jaw = ((prevJaw * (JawPeriods - 1)) + newVal) / JawPeriods;
            }
        }

        // calculate alligator's teeth, when in range
        if (i >= TeethPeriods + TeethOffset - 1)
        {
            // first/reset value: calculate SMA
            if (Cache[i - 1].Teeth is null)
            {
                double sum = 0;
                for (int p = i - TeethPeriods - TeethOffset + 1; p <= i - TeethOffset; p++)
                {
                    sum += Provider.Results[p].Hl2OrValue();
                }

                teeth = sum / TeethPeriods;
            }

            // remaining values: SMMA
            else
            {
                double prevTooth = Cache[i - 1].Teeth.Null2NaN();
                double newVal = Provider.Results[i - TeethOffset].Hl2OrValue();

                teeth = ((prevTooth * (TeethPeriods - 1)) + newVal) / TeethPeriods;
            }
        }

        // calculate alligator's lips, when in range
        if (i >= LipsPeriods + LipsOffset - 1)
        {
            // first/reset value: calculate SMA
            if (Cache[i - 1].Lips is null)
            {
                double sum = 0;
                for (int p = i - LipsPeriods - LipsOffset + 1; p <= i - LipsOffset; p++)
                {
                    sum += Provider.Results[p].Hl2OrValue();
                }

                lips = sum / LipsPeriods;
            }

            // remaining values: SMMA
            else
            {
                double prevLips = Cache[i - 1].Lips.Null2NaN();
                double newVal = Provider.Results[i - LipsOffset].Hl2OrValue();

                lips = ((prevLips * (LipsPeriods - 1)) + newVal) / LipsPeriods;
            }
        }

        // candidate result
        AlligatorResult r = new(
            newIn.Timestamp,
            jaw.NaN2Null(),
            teeth.NaN2Null(),
            lips.NaN2Null());

        // save and send
        Motify(act, r, i);
    }

    public override string ToString()
        => $"ALLIGATOR({JawPeriods},{JawOffset},{TeethPeriods},{TeethOffset},{LipsPeriods},{LipsOffset})";
}