namespace Regression;

[TestClass, TestCategory("Regression")]
public class StarcBandsTests : RegressionTestBase<StarcBandsResult>
{
    public StarcBandsTests() : base("starc.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToStarcBands().AssertEquals(Expected);

    [TestMethod]
    public override void Buffer() => Quotes.ToStarcBandsList().AssertEquals(Expected);

    [TestMethod]
    public override void Stream() => Quotes.ToStarcBandsHub().Results.AssertEquals(Expected);
}
