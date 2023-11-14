namespace Skender.Stock.Indicators;

// VOLUME WEIGHTED AVERAGE PRICE (STREAMING)

public partial class Vwap
{
    // TBD constructor
    public Vwap()
    {
        Initialize();
    }

    // TBD PROPERTIES

    // STATIC METHODS

    // parameter validation
    internal static void Validate(
        List<QuoteD> quotesList,
        DateTime? startDate)
    {
        // nothing to do for 0 length
        if (quotesList.Count == 0)
        {
            return;
        }

        // check parameter arguments (intentionally after quotes check)
        if (startDate < quotesList[0].Date)
        {
            throw new ArgumentOutOfRangeException(nameof(startDate), startDate,
                "Start Date must be within the quotes range for VWAP.");
        }
    }

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
