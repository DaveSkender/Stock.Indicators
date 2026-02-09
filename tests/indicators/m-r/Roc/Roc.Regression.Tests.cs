namespace Regression;

[TestClass, TestCategory("Regression")]
public class RocTests : RegressionTestBase<RocResult>
{
    public RocTests() : base("roc.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToRoc().IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => Quotes.ToRocList(14).IsExactly(Expected);

    [TestMethod]
    public override void Stream() => QuoteHub.ToRocHub(14).Results.IsExactly(Expected);
}
