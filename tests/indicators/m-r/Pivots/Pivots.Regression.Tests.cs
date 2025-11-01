namespace Regression;

[TestClass, TestCategory("Regression")]
public class PivotsTests : RegressionTestBase<PivotsResult>
{
    public PivotsTests() : base("pivots.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToPivots().AssertEquals(Expected);

    [TestMethod]
    public override void Buffer() => Quotes.ToPivotsList().AssertEquals(Expected);

    [TestMethod]
    public override void Stream()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(Quotes);
        PivotsHub hub = quoteHub.ToPivotsHub();
        hub.Rebuild(0);  // Calculate trend lines after all pivot points identified
        hub.Results.AssertEquals(Expected);
    }
}
