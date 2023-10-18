namespace Skender.Stock.Indicators;

// ACCUMULATION/DISTRIBUTION LINE (STREAMING)

public partial class Adl
{
    // constructor
    public Adl()
    {
        ProtectedResults = new();
        PrevAdl = 0;
    }

    // PROPERTIES

    public IEnumerable<AdlResult> Results => ProtectedResults;

    internal List<AdlResult> ProtectedResults { get; set; }

    private double PrevAdl { get; set; }
}
