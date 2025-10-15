namespace Tests.Data;

/// <summary>
/// Base setup for regression tests
/// </summary>
public abstract class RegressionTestBase<TResult>(string filename) : TestBase
    where TResult : ISeries
{
    protected IReadOnlyList<TResult> Expected { get; init; } = Data.Results<TResult>(filename);

    private static Skender.Stock.Indicators.QuoteHub CreateQuoteHub()
    {
        Skender.Stock.Indicators.QuoteHub hub = new();
        hub.Add(Quotes);
        return hub;
    }

    protected static Skender.Stock.Indicators.QuoteHub QuoteHub { get; } = CreateQuoteHub();

    public abstract void Series();
    public abstract void Buffer();
    public abstract void Stream();
}
