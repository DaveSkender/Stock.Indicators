namespace Regression;

[TestClass, TestCategory("Regression")]
public class HeikinashiTests : RegressionTestBase<HeikinAshiResult>
{
    public HeikinashiTests() : base("heikinashi.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToHeikinAshi().AssertEquals(Expected);

    [TestMethod]
    public override void Buffer() => Assert.Inconclusive("Buffer implementation not yet available");

    [TestMethod]
    public override void Stream() => Assert.Inconclusive("Stream implementation not yet available");
}
