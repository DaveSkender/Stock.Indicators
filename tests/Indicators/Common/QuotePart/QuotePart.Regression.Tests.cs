namespace Regression;

[TestClass, TestCategory("Regression")]
public class QuotePartTests : RegressionTestBase<QuotePart>
{
    public QuotePartTests() : base("quotepart.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToQuotePart(CandlePart.Close).IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => Quotes.ToQuotePartList(CandlePart.Close).IsExactly(Expected);

    [TestMethod]
    public override void Stream() => Quotes.ToQuotePartHub(CandlePart.Close).Results.IsExactly(Expected);
}
