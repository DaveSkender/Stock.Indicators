namespace Skender.Stock.Indicators;

// ACCUMULATION/DISTRIBUTION LINE (STREAMING)

/// <summary>See the <see href = "https://dotnet.stockindicators.dev/indicators/Adl/">
///  Stock Indicators for .NET online guide</see> for more information.</summary>
public partial class Adl : ChainProvider
{
    // TBD constructor
    public Adl()
    {
        Initialize();
    }

    // TBD PROPERTIES

    // STATIC METHODS

    // TBD parameter validation

    // increment calculation
    internal static (double mfm, double mfv, double adl) Increment(
        double lastAdl,
        double high,
        double low,
        double close,
        double volume)
    {
        double mfm = (high == low) ? 0 : (close - low - (high - close)) / (high - low);
        double mfv = mfm * volume;
        double adl = mfv + lastAdl;

        return (mfm, mfv, adl);
    }

    // NON-STATIC METHODS

    // handle quote arrival
    public override void OnNext((DateTime Date, double Value) value) => Add(value);

    // TBD add new tuple quote
    internal void Add((DateTime Date, double Value) tp) => throw new NotImplementedException();

    // TBD initialize with existing quote cache
    private void Initialize() => throw new NotImplementedException();
}
