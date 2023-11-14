namespace Skender.Stock.Indicators;

// RENKO CHART - STANDARD (STREAMING)

public partial class Renko
{
    // TBD constructor
    public Renko()
    {
        Initialize();
    }

    // TBD PROPERTIES

    // STATIC METHODS

    // parameter validation
    internal static void Validate(
        decimal brickSize)
    {
        // check parameter arguments
        if (brickSize <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(brickSize), brickSize,
                "Brick size must be greater than 0 for Renko Charts.");
        }
    }

    // TBD increment calculation
    internal static double Increment() => throw new NotImplementedException();

    // NON-STATIC METHODS

    // handle quote arrival
    public virtual void OnNext((DateTime Date, double Value) value)
    {
    }

    // TBD initialize with existing quote cache
    private void Initialize() => throw new NotImplementedException();
}
