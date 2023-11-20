namespace Skender.Stock.Indicators;

// BETA COEFFICIENT (STREAMING)

public partial class Beta
{
    // TBD: constructor
    public Beta()
    {
        Initialize();
    }

    // TBD: PROPERTIES

    // STATIC METHODS

    // parameter validation
    internal static void Validate(
        List<(DateTime, double)> tpListEval,
        List<(DateTime, double)> tpListMrkt,
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for Beta.");
        }

        // check quotes
        if (tpListEval.Count != tpListMrkt.Count)
        {
            throw new InvalidQuotesException(
                nameof(tpListEval),
                "Eval quotes should have the same number of Market quotes for Beta.");
        }
    }

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
