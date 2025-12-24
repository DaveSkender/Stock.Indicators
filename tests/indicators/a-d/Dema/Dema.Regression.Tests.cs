namespace Regression;

[TestClass, TestCategory("Regression")]
public class DemaTests : RegressionTestBase<DemaResult>
{
    public DemaTests() : base("dema.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToDema().IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => Assert.Inconclusive("Buffer implementation not yet available");

    [TestMethod]
    public override void Stream() => Assert.Inconclusive("Stream implementation not yet available");
}
