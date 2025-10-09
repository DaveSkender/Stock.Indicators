namespace Regression;

[TestClass, TestCategory("Regression")]
public class VolatilitystopTests : RegressionTestBase<VolatilityStopResult>
{
    public VolatilitystopTests() : base("vol-stop.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToVolatilityStop(14, 3).AssertEquals(Expected);

    [TestMethod]
    public override void Buffer() => Assert.Inconclusive("Buffer implementation not yet available");

    [TestMethod]
    public override void Stream() => Assert.Inconclusive("Stream implementation not yet available");
}
