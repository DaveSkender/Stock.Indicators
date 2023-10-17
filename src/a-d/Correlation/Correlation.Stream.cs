namespace Skender.Stock.Indicators;

// CORRELATION COEFFICIENT (STREAMING)

public partial class Correlation : ChainProvider
{
    // TBD constructor
    public Correlation()
    {
        Initialize();
    }

    // TBD PROPERTIES

    // STATIC METHODS

    // parameter validation
    internal static void Validate(
        List<(DateTime, double)> quotesA,
        List<(DateTime, double)> quotesB,
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for Correlation.");
        }

        // check quotes
        if (quotesA.Count != quotesB.Count)
        {
            throw new InvalidQuotesException(
                nameof(quotesB),
                "B quotes should have at least as many records as A quotes for Correlation.");
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
