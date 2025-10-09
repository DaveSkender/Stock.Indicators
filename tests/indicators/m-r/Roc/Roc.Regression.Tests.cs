namespace Regression;

[TestClass, TestCategory("Regression")]
public class RocTests : RegressionTestBase<RocResult>
{
    public RocTests() : base("roc.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToRoc(20).AssertEquals(Expected);

    [TestMethod]
    public override void Buffer() => Assert.Inconclusive("Buffer implementation not yet available");

    [TestMethod]
    public override void Stream() => Assert.Inconclusive("Stream implementation not yet available");
}
