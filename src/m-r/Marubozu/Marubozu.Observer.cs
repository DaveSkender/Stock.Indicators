namespace Skender.Stock.Indicators;

// MARUBOZU (STREAMING)

public partial class Marubozu : ChainProvider
{
    // TBD constructor
    public Marubozu()
    {
        Initialize();
    }

    // TBD PROPERTIES

    // STATIC METHODS

    // parameter validation
    internal static void Validate(
        double minBodyPercent)
    {
        // check parameter arguments
        if (minBodyPercent > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(minBodyPercent), minBodyPercent,
                "Minimum Body Percent must be less than 100 for Marubozu (<=100%).");
        }

        if (minBodyPercent < 80)
        {
            throw new ArgumentOutOfRangeException(nameof(minBodyPercent), minBodyPercent,
                "Minimum Body Percent must at least 80 (80%) for Marubozu and is usually greater than 90 (90%).");
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
