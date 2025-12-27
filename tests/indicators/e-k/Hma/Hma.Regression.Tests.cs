namespace Regression;

[TestClass, TestCategory("Regression")]
public class HmaTests : RegressionTestBase<HmaResult>
{
    public HmaTests() : base("hma.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToHma().IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => new HmaList(14) { Quotes }.IsExactly(Expected);

    [TestMethod]
    public override void Stream() => QuoteHub.ToHmaHub(14).Results.IsExactly(Expected);
}
