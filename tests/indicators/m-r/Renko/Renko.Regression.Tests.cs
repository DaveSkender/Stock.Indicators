namespace Regression;

[TestClass, TestCategory("Regression")]
public class RenkoTests : RegressionTestBase<RenkoResult>
{
    public RenkoTests() : base("renko.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToRenko(1.0m).IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => new RenkoList(1.0m) { Quotes }.IsExactly(Expected);

    [TestMethod]
    public override void Stream() => Assert.Inconclusive("Stream implementation not yet available");
}
