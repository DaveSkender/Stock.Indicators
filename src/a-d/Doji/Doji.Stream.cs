namespace Skender.Stock.Indicators;

// DOJI (STREAMING)

public partial class Doji
{
    // TBD: constructor
    public Doji()
    {
        Initialize();
    }

    // TBD: PROPERTIES

    // STATIC METHODS

    // parameter validation
    internal static void Validate(
        double maxPriceChangePercent)
    {
        // check parameter arguments
        if (maxPriceChangePercent is < 0 or > 0.5)
        {
            throw new ArgumentOutOfRangeException(nameof(maxPriceChangePercent), maxPriceChangePercent,
                "Maximum Percent Change must be between 0 and 0.5 for Doji (0% to 0.5%).");
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
