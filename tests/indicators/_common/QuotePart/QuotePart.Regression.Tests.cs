namespace Regression;

[TestClass, TestCategory("Regression")]
public class QuotePartTests : RegressionTestBase<TimeValue>
{
    public QuotePartTests() : base("quotepart.standard.json") { }

    [TestMethod]
    public override void Series_AgainstBaseline_MatchesExactly() => Quotes.ToQuotePart(CandlePart.Close).IsExactly(Expected);

    [TestMethod]
    public override void Buffer_AgainstBaseline_MatchesExactly() => Quotes.ToQuotePartList(CandlePart.Close).IsExactly(Expected);

    [TestMethod]
    public override void Stream_AgainstBaseline_MatchesExactly() => Quotes.ToQuotePartHub(CandlePart.Close).Results.IsExactly(Expected);
}
