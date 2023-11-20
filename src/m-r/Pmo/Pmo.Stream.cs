namespace Skender.Stock.Indicators;

// PRICE MOMENTUM OSCILLATOR (STREAMING)

public partial class Pmo
{
    // TBD: constructor
    public Pmo()
    {
        Initialize();
    }

    // TBD: PROPERTIES

    // STATIC METHODS

    // parameter validation
    internal static void Validate(
        int timePeriods,
        int smoothPeriods,
        int signalPeriods)
    {
        // check parameter arguments
        if (timePeriods <= 1)
        {
            throw new ArgumentOutOfRangeException(nameof(timePeriods), timePeriods,
                "Time periods must be greater than 1 for PMO.");
        }

        if (smoothPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(smoothPeriods), smoothPeriods,
                "Smoothing periods must be greater than 0 for PMO.");
        }

        if (signalPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(signalPeriods), signalPeriods,
                "Signal periods must be greater than 0 for PMO.");
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
