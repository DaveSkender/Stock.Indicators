namespace Regression;

[TestClass, TestCategory("Regression")]
public class KeltnerTests : RegressionTestBase<KeltnerResult>
{
    public KeltnerTests() : base("keltner.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToKeltner(20, 2, 10).IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => Quotes.ToKeltnerList(20, 2, 10).IsExactly(Expected);

    [TestMethod]
    public override void Stream() => QuoteHub.ToKeltnerHub(20, 2, 10).Results.IsExactly(Expected);
}
