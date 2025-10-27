namespace Tests.Data;

/// <summary>
/// Base setup for regression tests
/// </summary>
/// <param name="filename"></param>
public abstract class RegressionTestBase<TResult>(string filename) : TestBase
    where TResult : ISeries
{
    protected IReadOnlyList<TResult> Expected { get; init; } = Data.Results<TResult>(filename);

    private static QuoteHub CreateQuoteHub()
    {
        QuoteHub hub = new();
        hub.Add(Quotes);
        return hub;
    }

    protected static QuoteHub QuoteHub { get; } = CreateQuoteHub();

    public abstract void Series();
    public abstract void Buffer();
    public abstract void Stream();
}
