namespace Regression;

[TestClass, TestCategory("Regression")]
public class RocWbTests : RegressionTestBase<RocWbResult>
{
    public RocWbTests() : base("roc-wb.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToRocWb().AssertEquals(Expected);

    [TestMethod]
    public override void Buffer() => Quotes.ToRocWbList().AssertEquals(Expected);

    [TestMethod]
    public override void Stream() => Assert.Inconclusive("Stream implementation not yet available");
}
