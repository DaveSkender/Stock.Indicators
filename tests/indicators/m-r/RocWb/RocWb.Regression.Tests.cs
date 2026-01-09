namespace Regression;

[TestClass, TestCategory("Regression")]
public class RocWbTests : RegressionTestBase<RocWbResult>
{
    public RocWbTests() : base("roc-wb.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToRocWb().IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => Quotes.ToRocWbList().IsExactly(Expected);

    [TestMethod]
    public override void Stream() => Quotes.ToRocWbHub().Results.IsExactly(Expected);
}
