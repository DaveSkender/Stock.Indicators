namespace Skender.Stock.Indicators;

// ACCUMULATION/DISTRIBUTION LINE (STREAMING)

public partial class Adl : ChainProvider
{
    // TBD constructor
    public Adl()
    {
        Initialize();
    }

    // TBD PROPERTIES

    // STATIC METHODS

    // TBD parameter validation

    // parameter validation
    internal static void Validate(
        int? smaPeriods)
    {
        // check parameter arguments
        if (smaPeriods is not null and <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(smaPeriods), smaPeriods,
                "SMA periods must be greater than 0 for ADL.");
        }
    }

    // increment calculation
    internal static (double mfm, double mfv, double adl) Increment(
        double lastAdl,
        QuoteD q)
    {
        double mfm = (q.High == q.Low) ? 0 : (q.Close - q.Low - (q.High - q.Close)) / (q.High - q.Low);
        double mfv = mfm * q.Volume;
        double adl = mfv + lastAdl;

        return (mfm, mfv, adl);
    }

    // NON-STATIC METHODS

    // handle quote arrival
    public override void OnNext((DateTime Date, double Value) value) => Add(value);

    // TBD add new tuple quote
    internal void Add((DateTime Date, double Value) tp) => throw new NotImplementedException();

    // TBD initialize with existing quote cache
    private void Initialize() => throw new NotImplementedException();
}
