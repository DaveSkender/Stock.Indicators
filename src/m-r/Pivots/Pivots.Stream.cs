namespace Skender.Stock.Indicators;

// PIVOTS (STREAMING)

public partial class Pivots : ChainProvider
{
    // TBD constructor
    public Pivots()
    {
        Initialize();
    }

    // TBD PROPERTIES

    // STATIC METHODS

    // parameter validation
    internal static void Validate(
        int leftSpan,
        int rightSpan,
        int maxTrendPeriods,
        string caller = "Pivots")
    {
        // check parameter arguments
        if (rightSpan < 2)
        {
            throw new ArgumentOutOfRangeException(nameof(rightSpan), rightSpan,
                $"Right span must be at least 2 for {caller}.");
        }

        if (leftSpan < 2)
        {
            throw new ArgumentOutOfRangeException(nameof(leftSpan), leftSpan,
                $"Left span must be at least 2 for {caller}.");
        }

        if (maxTrendPeriods <= leftSpan)
        {
            throw new ArgumentOutOfRangeException(nameof(leftSpan), leftSpan,
                $"Lookback periods must be greater than the Left window span for {caller}.");
        }
    }

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
