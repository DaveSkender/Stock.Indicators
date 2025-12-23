namespace Regression;

[TestClass, TestCategory("Regression")]
public class UltimateTests : RegressionTestBase<UltimateResult>
{
    public UltimateTests() : base("uo.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToUltimate(7, 14, 28).IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => Assert.Inconclusive("Buffer implementation not yet available");

    [TestMethod]
    public override void Stream() => Assert.Inconclusive("Stream implementation not yet available");
}
