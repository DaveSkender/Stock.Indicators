namespace Regression;

[TestClass, TestCategory("Regression")]
public class DojiTests : RegressionTestBase<CandleResult>
{
    public DojiTests() : base("doji.standard.json") { }

    [TestMethod]
    public override void Series_AgainstBaseline_MatchesExactly() => Quotes.ToDoji(0.1).IsExactly(Expected);

    [TestMethod]
    public override void Buffer_AgainstBaseline_MatchesExactly() => Quotes.ToDojiList(0.1).IsExactly(Expected);

    [TestMethod]
    public override void Stream_AgainstBaseline_MatchesExactly() => QuoteHub.ToDojiHub(0.1).Results.IsExactly(Expected);
}
