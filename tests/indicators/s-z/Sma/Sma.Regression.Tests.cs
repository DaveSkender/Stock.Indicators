namespace Regression;

[TestClass, TestCategory("Regression")]
public class SmaTests : RegressionTestBase<SmaResult>
{
    public SmaTests() : base("sma.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToSma(20).IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => Quotes.ToSmaList(20).IsExactly(Expected);

    [TestMethod]
    public override void Stream() => QuoteHub.ToSmaHub(20).Results.IsExactly(Expected);
}
