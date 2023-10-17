namespace Skender.Stock.Indicators;

// MOVING AVERAGE ENVELOPES (STREAMING)

public partial class MaEnvelopes : ChainProvider
{
    // TBD constructor
    public MaEnvelopes()
    {
        Initialize();
    }

    // TBD PROPERTIES

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

    // TBD increment calculation
    internal static double Increment() => throw new NotImplementedException();

    // NON-STATIC METHODS

    // handle quote arrival
    public override void OnNext((DateTime Date, double Value) value) => Add(value);

    // TBD add new tuple quote
    internal void Add((DateTime Date, double Value) tp) => throw new NotImplementedException();

    // TBD initialize with existing quote cache
    private void Initialize() => throw new NotImplementedException();
}
