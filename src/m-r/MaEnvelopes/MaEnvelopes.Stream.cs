namespace Skender.Stock.Indicators;

// MOVING AVERAGE ENVELOPES (STREAMING)

public partial class MaEnvelopes
{
    // TBD: constructor
    public MaEnvelopes()
    {
        Initialize();
    }

    // TBD: PROPERTIES

    // STATIC METHODS

    // parameter validation
    internal static void Validate(
        double percentOffset)
    {
        // check parameter arguments
        if (percentOffset <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(percentOffset), percentOffset,
                "Percent Offset must be greater than 0 for Moving Average Envelopes.");
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
