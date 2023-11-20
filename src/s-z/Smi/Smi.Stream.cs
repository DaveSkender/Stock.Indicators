namespace Skender.Stock.Indicators;

// STOCHASTIC MOMENTUM INDEX (STREAMING)

public partial class Smi
{
    // TBD: constructor
    public Smi()
    {
        Initialize();
    }

    // TBD: PROPERTIES

    // STATIC METHODS

    // parameter validation
    internal static void Validate(
        int lookbackPeriods,
        int firstSmoothPeriods,
        int secondSmoothPeriods,
        int signalPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for SMI.");
        }

        if (firstSmoothPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(firstSmoothPeriods), firstSmoothPeriods,
                "Smoothing periods must be greater than 0 for SMI.");
        }

        if (secondSmoothPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(secondSmoothPeriods), secondSmoothPeriods,
                "Smoothing periods must be greater than 0 for SMI.");
        }

        if (signalPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(signalPeriods), signalPeriods,
                "Signal periods must be greater than 0 for SMI.");
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
