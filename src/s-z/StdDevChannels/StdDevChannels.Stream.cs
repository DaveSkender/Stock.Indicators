namespace Skender.Stock.Indicators;

// STANDARD DEVIATION CHANNELS (STREAMING)

public partial class StdDevChannels
{
    // TBD: constructor
    public StdDevChannels()
    {
        Initialize();
    }

    // TBD: PROPERTIES

    // STATIC METHODS

    // parameter validation
    internal static void Validate(
        int? lookbackPeriods,
        double stdDeviations)
    {
        // check parameter arguments
        if (lookbackPeriods <= 1)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 1 for Standard Deviation Channels.");
        }

        if (stdDeviations <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(stdDeviations), stdDeviations,
                "Standard Deviations must be greater than 0 for Standard Deviation Channels.");
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
