namespace Skender.Stock.Indicators;

// ZIG ZAG (STREAMING)

public partial class ZigZag
{
    // TBD: constructor
    public ZigZag()
    {
        Initialize();
    }

    // TBD: PROPERTIES

    // STATIC METHODS

    // parameter validation
    internal static void Validate(
        decimal percentChange)
    {
        // check parameter arguments
        if (percentChange <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(percentChange), percentChange,
                "Percent change must be greater than 0 for ZIGZAG.");
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
