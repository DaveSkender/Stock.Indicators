namespace Regression;

[TestClass, TestCategory("Regression")]
public class AtrTests : RegressionTestBase<AtrResult>
{
    public AtrTests() : base("atr.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToAtr(14).IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => new AtrList(14) { Quotes }.IsExactly(Expected);

    [TestMethod]
    public override void Stream() => QuoteHub.ToAtrHub(14).Results.IsExactly(Expected);
}
