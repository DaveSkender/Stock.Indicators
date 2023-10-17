namespace Skender.Stock.Indicators;

// BASE QUOTE (STREAMING)

public partial class BasicQuote : ChainProvider
{
    // TBD constructor
    public BasicQuote()
    {
        Initialize();
    }

    // TBD PROPERTIES

    // STATIC METHODS

    // TBD parameter validation
    internal static void Validate(Quote quote)
    {
        if (quote is null)
        {
            throw new ArgumentNullException(nameof(quote), "Quote cannot be null for BasicQuotes");
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
