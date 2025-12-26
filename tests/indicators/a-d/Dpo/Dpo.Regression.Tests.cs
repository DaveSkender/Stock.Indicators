namespace Regression;

[TestClass, TestCategory("Regression")]
public class DpoTests : RegressionTestBase<DpoResult>
{
    public DpoTests() : base("dpo.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToDpo().IsExactly(Expected);

    [TestMethod]
    public override void Buffer()
    {
        DpoList list = Quotes.ToDpoList(14);
        list.IsExactly(Expected.Take(list.Count));
    }

    [TestMethod]
    public override void Stream()
    {
        QuoteHub quoteHub = new();
        DpoHub hub = quoteHub.ToDpoHub(14);

        // Add quotes to the hub
        foreach (Quote q in Quotes)
        {
            quoteHub.Add(q);
        }

        // Get results
        IReadOnlyList<DpoResult> actuals = hub.Results;

        // Verify results match expected
        actuals.IsExactly(Expected);

        // Cleanup
        hub.Unsubscribe();
        quoteHub.EndTransmission();
    }
}
