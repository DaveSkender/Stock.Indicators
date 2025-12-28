namespace Regression;

[TestClass, TestCategory("Regression")]
public class KamaTests : RegressionTestBase<KamaResult>
{
    public KamaTests() : base("kama.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToKama(10, 2, 30).IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => Quotes.ToKamaList(10, 2, 30).IsExactly(Expected);

    [TestMethod]
    public override void Stream() => QuoteHub.ToKamaHub(10, 2, 30).Results.IsExactly(Expected);
}
