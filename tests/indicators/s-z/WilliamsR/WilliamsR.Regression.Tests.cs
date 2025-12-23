namespace Regression;

[TestClass, TestCategory("Regression")]
public class WilliamsrTests : RegressionTestBase<WilliamsResult>
{
    public WilliamsrTests() : base("willr.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToWilliamsR(14).IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => Assert.Inconclusive("Buffer implementation not yet available");

    [TestMethod]
    public override void Stream() => Assert.Inconclusive("Stream implementation not yet available");
}
