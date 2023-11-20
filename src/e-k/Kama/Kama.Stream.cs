namespace Skender.Stock.Indicators;

// KAUFMAN's ADAPTIVE MOVING AVERAGE (STREAMING)

public partial class Kama
{
    // TBD: constructor
    public Kama()
    {
        Initialize();
    }

    // TBD: PROPERTIES

    // STATIC METHODS

    // parameter validation
    internal static void Validate(
        int erPeriods,
        int fastPeriods,
        int slowPeriods)
    {
        // check parameter arguments
        if (erPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(erPeriods), erPeriods,
                "Efficiency Ratio periods must be greater than 0 for KAMA.");
        }

        if (fastPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(fastPeriods), fastPeriods,
                "Fast EMA periods must be greater than 0 for KAMA.");
        }

        if (slowPeriods <= fastPeriods)
        {
            throw new ArgumentOutOfRangeException(nameof(slowPeriods), slowPeriods,
                "Slow EMA periods must be greater than Fast EMA period for KAMA.");
        }
    }

    // TBD: increment  calculation
    internal static double Increment() => throw new NotImplementedException();

    // NON-STATIC METHODS

    // handle quote arrival
    public virtual void OnNext((DateTime Date, double Value) value)
    {
    }

    // TBD: initialize with existing quote cache
    private void Initialize() => throw new NotImplementedException();
}
