namespace Skender.Stock.Indicators;

public partial class Alligator : ChainObserver<AlligatorResult>, IAlligator
{
    // constructor
    public Alligator(
        ChainProvider provider,
        int jawPeriods,
        int jawOffset,
        int teethPeriods,
        int teethOffset,
        int lipsPeriods,
        int lipsOffset)
        : base(provider)
    {
        Validate(
            jawPeriods,
            jawOffset,
            teethPeriods,
            teethOffset,
            lipsPeriods,
            lipsOffset);

        JawPeriods = jawPeriods;
        JawOffset = jawOffset;
        TeethPeriods = teethPeriods;
        TeethOffset = teethOffset;
        LipsPeriods = lipsPeriods;
        LipsOffset = lipsOffset;

        RebuildCache();

        // subscribe to provider
        unsubscriber = provider != null
           ? provider.Subscribe(this)
           : throw new ArgumentNullException(nameof(provider));
    }

    // PROPERTIES

    public int JawPeriods { get; private set; }
    public int JawOffset { get; private set; }
    public int TeethPeriods { get; private set; }
    public int TeethOffset { get; private set; }
    public int LipsPeriods { get; private set; }
    public int LipsOffset { get; private set; }

    // METHODS

    // handle chain arrival
    public override void OnNext((Act act, DateTime date, double price) value)
    {
        int i;
        double jaw = double.NaN;
        double lips = double.NaN;
        double teeth = double.NaN;

        List<(DateTime _, double value)> supplier = ChainSupplier.Chain;

        // handle deletes
        if (value.act == Act.Delete)
        {
            i = Cache.FindIndex(value.date);
            AlligatorResult alligator = Cache[i];
            jaw = alligator.Jaw.Null2NaN();
            lips = alligator.Lips.Null2NaN();
            teeth = alligator.Teeth.Null2NaN();
        }

        // handle new values
        else
        {

            i = ChainSupplier.Chain.FindIndex(value.date);

            // source unexpectedly not found
            if (i == -1)
            {
                throw new InvalidOperationException(
                    "Matching source history not found on arrival.");
            }

            // calculate alligator's jaw, when in range
            if (i >= JawPeriods + JawOffset - 1)
            {
                double prevJaw = Cache[i - 1].Jaw.Null2NaN();

                // first/reset value: calculate SMA
                if (double.IsNaN(prevJaw))
                {
                    double sum = 0;
                    for (int p = i - JawPeriods - JawOffset + 1; p <= i - JawOffset; p++)
                    {
                        sum += ChainSupplier.Chain[p].Value;
                    }

                    jaw = sum / JawPeriods;
                }

                // remaining values: SMMA
                else
                {
                    double newVal = ChainSupplier.Chain[i - JawOffset].Value;
                    jaw = ((prevJaw * (JawPeriods - 1)) + newVal) / JawPeriods;
                }
            }

            // calculate alligator's teeth, when in range
            if (i >= TeethPeriods + TeethOffset - 1)
            {
                double prevTeeth = Cache[i - 1].Teeth.Null2NaN();

                // first/reset value: calculate SMA
                if (double.IsNaN(prevTeeth))
                {
                    double sum = 0;
                    for (int p = i - TeethPeriods - TeethOffset + 1; p <= i - TeethOffset; p++)
                    {

                        sum += ChainSupplier.Chain[p].Value;
                    }

                    teeth = sum / TeethPeriods;
                }

                // remaining values: SMMA
                else
                {
                    double newVal = ChainSupplier.Chain[i - TeethOffset].Value;
                    teeth = ((prevTeeth * (TeethPeriods - 1)) + newVal) / TeethPeriods;
                }
            }

            // calculate alligator's lips, when in range
            if (i >= LipsPeriods + LipsOffset - 1)
            {
                double prevLips = Cache[i - 1].Lips.Null2NaN();

                // first/reset value: calculate SMA
                if (double.IsNaN(prevLips))
                {
                    double sum = 0;
                    for (int p = i - LipsPeriods - LipsOffset + 1; p <= i - LipsOffset; p++)
                    {
                        sum += ChainSupplier.Chain[p].Value;
                    }

                    lips = sum / LipsPeriods;
                }

                // remaining values: SMMA
                else
                {
                    double newVal = ChainSupplier.Chain[i - LipsOffset].Value;
                    lips = ((prevLips * (LipsPeriods - 1)) + newVal) / LipsPeriods;
                }
            }
        }

        // candidate result
        AlligatorResult r = new() {
            Timestamp = value.date,
            Jaw = jaw.NaN2Null(),
            Lips = lips.NaN2Null(),
            Teeth = teeth.NaN2Null()
        };

        // save to cache
        Act act = CacheResultPerAction(value.act, r);

        // note: this indicator is not observable (no notification)

        // update forward values
        if (act != Act.AddNew && i < supplier.Count - 1)
        {
            // cascade updates gracefully
            int next = act == Act.Delete ? i : i + 1;
            (DateTime d, double v) = supplier[next];
            OnNext((Act.Update, d, v));
        }
    }

    // delete cache between index values
    // usually called from inherited ClearCache(fromDate)
    internal override void ClearCache(int fromIndex, int toIndex)
    {
        // delete and deliver instruction,
        // in reverse order to prevent recompositions

        for (int i = toIndex; i >= fromIndex; i--)
        {
            AlligatorResult r = Cache[i];
            Act act = CacheResultPerAction(Act.Delete, r);
        }
    }
}
