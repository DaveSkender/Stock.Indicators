namespace Regression;

[TestClass, TestCategory("Regression")]
public class IchimokuTests : RegressionTestBase<IchimokuResult>
{
    public IchimokuTests() : base("ichimoku.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToIchimoku(9, 26, 52).IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => Quotes.ToIchimokuList(9, 26, 52).IsExactly(Expected);

    [TestMethod]
    public override void Stream() => QuoteHub.ToIchimokuHub(9, 26, 52).Results.IsExactly(Expected);
}
