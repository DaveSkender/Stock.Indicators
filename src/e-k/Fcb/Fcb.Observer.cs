namespace Skender.Stock.Indicators;

// Fcb (STREAMING)

public partial class Fcb : ChainProvider
{
    // TBD constructor
    public Fcb()
    {
        Initialize();
    }

    // TBD PROPERTIES

    // STATIC METHODS

    // TBD parameter validation
    internal static void Validate() => throw new NotImplementedException();

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
