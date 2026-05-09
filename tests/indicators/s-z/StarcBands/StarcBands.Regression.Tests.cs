namespace Regression;

[TestClass, TestCategory("Regression")]
public class StarcBandsTests : RegressionTestBase<StarcBandsResult>
{
    public StarcBandsTests() : base("starc.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToStarcBands().IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => Quotes.ToStarcBandsList().IsExactly(Expected);

    [TestMethod]
    public override void Stream() => QuoteHub.ToStarcBandsHub().Results.IsExactly(Expected);
}
