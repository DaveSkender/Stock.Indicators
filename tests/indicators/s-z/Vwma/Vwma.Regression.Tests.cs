namespace Regression;

[TestClass, TestCategory("Regression")]
public class VwmaTests : RegressionTestBase<VwmaResult>
{
    public VwmaTests() : base("vwma.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToVwma(14).IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => Assert.Inconclusive("Buffer implementation not yet available");

    [TestMethod]
    public override void Stream() => Assert.Inconclusive("Stream implementation not yet available");
}
