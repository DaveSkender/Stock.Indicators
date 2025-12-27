namespace Regression;

[TestClass, TestCategory("Regression")]
public class DemaTests : RegressionTestBase<DemaResult>
{
    public DemaTests() : base("dema.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToDema().IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => new DemaList(14) { Quotes }.IsExactly(Expected);

    [TestMethod]
    public override void Stream() => QuoteHub.ToDemaHub(14).Results.IsExactly(Expected);
}
