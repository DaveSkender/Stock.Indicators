namespace Skender.Stock.Indicators;

// PIVOT POINTS (STREAMING)

public partial class PivotPoints
{
    // TBD: constructor
    public PivotPoints()
    {
        Initialize();
    }

    // TBD: PROPERTIES

    // STATIC METHODS

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
