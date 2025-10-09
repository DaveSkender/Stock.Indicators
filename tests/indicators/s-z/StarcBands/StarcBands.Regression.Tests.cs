namespace Regression;

[TestClass, TestCategory("Regression")]
public class StarcbandsTests : RegressionTestBase<StarcBandsResult>
{
    public StarcbandsTests() : base("starc.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToStarcBands().AssertEquals(Expected);

    [TestMethod]
    public override void Buffer() => Assert.Inconclusive("Buffer implementation not yet available");

    [TestMethod]
    public override void Stream() => Assert.Inconclusive("Stream implementation not yet available");
}
