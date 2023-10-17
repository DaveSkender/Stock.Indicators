namespace Skender.Stock.Indicators;

// TRUE RANGE (STREAMING)

public partial class Tr : ChainProvider
{
    // TBD constructor
    public Tr()
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
