namespace Regression;

[TestClass, TestCategory("Regression")]
public class PivotsTests : RegressionTestBase<PivotsResult>
{
    public PivotsTests() : base("pivots.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToPivots().IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => Quotes.ToPivotsList().IsExactly(Expected);

    [TestMethod]
    public override void Stream()
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(Quotes);
        PivotsHub hub = quoteHub.ToPivotsHub();
        hub.Rebuild(0);  // Calculate trend lines after all pivot points identified
        hub.Results.IsExactly(Expected);
    }
}
