namespace Skender.Stock.Indicators;

// AVERAGE DIRECTIONAL INDEX (STREAMING)

public partial class Adx : ChainProvider
{
    // TBD constructor
    public Adx()
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
