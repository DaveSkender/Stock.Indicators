namespace Skender.Stock.Indicators;

// SCHAFF TREND CYCLE (STREAMING)

public partial class Stc
{
    // TBD: constructor
    public Stc()
    {
        Initialize();
    }

    // TBD: PROPERTIES

    // STATIC METHODS

    // parameter validation
    internal static void Validate(
        int cyclePeriods,
        int fastPeriods,
        int slowPeriods)
    {
        // check parameter arguments
        if (cyclePeriods < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(cyclePeriods), cyclePeriods,
                "Trend Cycle periods must be greater than or equal to 0 for STC.");
        }

        if (fastPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(fastPeriods), fastPeriods,
                "Fast periods must be greater than 0 for STC.");
        }

        if (slowPeriods <= fastPeriods)
        {
            throw new ArgumentOutOfRangeException(nameof(slowPeriods), slowPeriods,
                "Slow periods must be greater than the fast period for STC.");
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
