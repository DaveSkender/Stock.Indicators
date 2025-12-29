namespace Regression;

[TestClass, TestCategory("Regression")]
public class EmaTests : RegressionTestBase<EmaResult>
{
    public EmaTests() : base("ema.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToEma(20).IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => Quotes.ToEmaList(20).IsExactly(Expected);

    [TestMethod]
    public override void Stream() => QuoteHub.ToEmaHub(20).Results.IsExactly(Expected);
}
