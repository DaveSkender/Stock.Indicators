namespace Regression;

[TestClass, TestCategory("Regression")]
public class AtrTests : RegressionTestBase<AtrResult>
{
    public AtrTests() : base("atr.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToAtr(14).IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => Quotes.ToAtrList(14).IsExactly(Expected);

    [TestMethod]
    public override void Stream() => QuoteHub.ToAtrHub(14).Results.IsExactly(Expected);
}
