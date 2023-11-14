namespace Skender.Stock.Indicators;

// TRUE RANGE (STREAMING)

public partial class Tr
{
    // TBD constructor
    public Tr()
    {
        Initialize();
    }

    // TBD PROPERTIES

    // METHODS

    // handle quote arrival
    public virtual void OnNext((DateTime Date, double Value) value)
    {
    }

    // TBD initialize with existing quote cache
    private void Initialize() => throw new NotImplementedException();
}
