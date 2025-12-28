namespace Regression;

[TestClass, TestCategory("Regression")]
public class QuotePartTests : RegressionTestBase<QuotePart>
{
    public QuotePartTests() : base("quotepart.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToQuotePart(CandlePart.Close).IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => new QuotePartList(CandlePart.Close) { Quotes }.IsExactly(Expected);

    [TestMethod]
    public override void Stream() => QuoteHub.ToQuotePartHub(CandlePart.Close).Results.IsExactly(Expected);
}
