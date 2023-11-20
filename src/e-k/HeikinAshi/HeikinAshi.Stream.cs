namespace Skender.Stock.Indicators;

// HEIKIN-ASHI (STREAMING)

public partial class HeikinAshi
{
    // TBD: constructor
    public HeikinAshi()
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
