namespace Skender.Stock.Indicators;

// ACCUMULATION/DISTRIBUTION LINE (STREAMING)

public partial class Adl : ChainProvider
{
    // TBD constructor
    public Adl()
    {
        Initialize();
    }

    // TBD PROPERTIES

    // METHODS

    // handle quote arrival
    public override void OnNext((DateTime Date, double Value) value) => Add(value);

    // TBD add new tuple quote
    internal void Add((DateTime Date, double Value) tp) => throw new NotImplementedException();

    // TBD initialize with existing quote cache
    private void Initialize() => throw new NotImplementedException();
}
