namespace Skender.Stock.Indicators;

public class Alligator<TIn>
    : AbstractChainInResultOut<TIn, AlligatorResult>, IAlligator
    where TIn : struct, IReusableResult
{
    // constructor
    public Alligator(
        IChainProvider<TIn> provider,
        int jawPeriods,
        int jawOffset,
        int teethPeriods,
        int teethOffset,
        int lipsPeriods,
        int lipsOffset)
        : base(provider)
    {
        Alligator.Validate(
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

        // subscribe to provider
        Subscription = provider != null
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

    // string label
    public override string ToString()
        => $"ALLIGATOR({JawPeriods},{JawOffset},{TeethPeriods},{TeethOffset},{LipsPeriods},{LipsOffset})";

    // handle chain arrival
    internal override void OnNextArrival(Act act, IReusableResult inbound)
    {
        int i;
        double jaw = double.NaN;
        double lips = double.NaN;
        double teeth = double.NaN;

        // handle deletes
        if (act == Act.Delete)
        {
            i = Cache.FindIndex(inbound.Timestamp);
            AlligatorResult alligator = Cache[i];

            jaw = alligator.Jaw.Null2NaN();
            lips = alligator.Lips.Null2NaN();
            teeth = alligator.Teeth.Null2NaN();
        }

        // handle new values
        else
        {

            i = Provider.FindIndex(inbound.Timestamp);

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
                        sum += ToValue(ProviderCache[p]);
                    }

                    jaw = sum / JawPeriods;
                }

                // remaining values: SMMA
                else
                {
                    double newVal = ToValue(ProviderCache[i - JawOffset]);
                    jaw = ((prevJaw * (JawPeriods - 1)) + newVal) / JawPeriods;
                }
            }

            // calculate alligator's teeth, when in range
            if (i >= TeethPeriods + TeethOffset - 1)
            {
                double prevTooth = Cache[i - 1].Teeth.Null2NaN();

                // first/reset value: calculate SMA
                if (double.IsNaN(prevTooth))
                {
                    double sum = 0;
                    for (int p = i - TeethPeriods - TeethOffset + 1; p <= i - TeethOffset; p++)
                    {
                        sum += ToValue(ProviderCache[p]);
                    }

                    teeth = sum / TeethPeriods;
                }

                // remaining values: SMMA
                else
                {
                    double newVal = ToValue(ProviderCache[i - TeethOffset]);
                    teeth = ((prevTooth * (TeethPeriods - 1)) + newVal) / TeethPeriods;
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
                        sum += ToValue(ProviderCache[p]);
                    }

                    lips = sum / LipsPeriods;
                }

                // remaining values: SMMA
                else
                {
                    double newVal = ToValue(ProviderCache[i - LipsOffset]);
                    lips = ((prevLips * (LipsPeriods - 1)) + newVal) / LipsPeriods;
                }
            }
        }

        // candidate result
        AlligatorResult r = new() {
            Timestamp = inbound.Timestamp,
            Jaw = jaw.NaN2Null(),
            Lips = lips.NaN2Null(),
            Teeth = teeth.NaN2Null()
        };

        // save to cache
        act = ModifyCache(act, r);

        // send to observers
        NotifyObservers(act, r);

        // cascade update forward values (recursively)
        if (act != Act.AddNew && i < ProviderCache.Count - 1)
        {
            int next = act == Act.Delete ? i : i + 1;
            TIn value = ProviderCache[next];
            OnNextArrival(Act.Update, value);
        }
    }

    // convert provider IQuotes to HL2, if needed
    private readonly Func<TIn, double> ToValue
        = (input) => input is IQuote quote
        ? quote.ToReusable(CandlePart.HL2).Value
        : input.Value;
}
