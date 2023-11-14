namespace Skender.Stock.Indicators;

// GATOR OSCILLATOR (STREAMING)

public partial class Gator
{
    // TBD constructor
    public Gator()
    {
        Initialize();
    }

    // TBD PROPERTIES

    // STATIC METHODS

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
