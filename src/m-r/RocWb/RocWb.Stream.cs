namespace Skender.Stock.Indicators;

// RocWb (STREAMING)

public partial class RocWb
{
    // TBD: constructor
    public RocWb()
    {
        Initialize();
    }

    // TBD: PROPERTIES

    // STATIC METHODS

    // parameter validation
    internal static void Validate(
        int lookbackPeriods,
        int emaPeriods,
        int stdDevPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for ROC with Bands.");
        }

        if (emaPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(emaPeriods), emaPeriods,
                "EMA periods must be greater than 0 for ROC.");
        }

        if (stdDevPeriods <= 0 || stdDevPeriods > lookbackPeriods)
        {
            throw new ArgumentOutOfRangeException(nameof(stdDevPeriods), stdDevPeriods,
                "Standard Deviation periods must be greater than 0 and less than lookback period for ROC with Bands.");
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
