namespace Regression;

[TestClass, TestCategory("Regression")]
public class VolatilitystopTests : RegressionTestBase<VolatilityStopResult>
{
    public VolatilitystopTests() : base("vol-stop.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToVolatilityStop().AssertEquals(Expected);

    [TestMethod]
    public override void Buffer() => new VolatilityStopList(7, 3, Quotes).AssertEquals(Expected);

    [TestMethod]
    public override void Stream() => Assert.Inconclusive("Stream implementation not yet available");
}
