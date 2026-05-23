namespace Regression;

[TestClass, TestCategory("Regression")]
public class AwesomeTests : RegressionTestBase<AwesomeResult>
{
    public AwesomeTests() : base("awesome.standard.json") { }

    [TestMethod]
    public override void Series_AgainstBaseline_MatchesExactly() => Quotes.ToAwesome(5, 34).IsExactly(Expected);

    [TestMethod]
    public override void Buffer_AgainstBaseline_MatchesExactly() => Quotes.ToAwesomeList(5, 34).IsExactly(Expected);

    [TestMethod]
    public override void Stream_AgainstBaseline_MatchesExactly() => QuoteHub.ToAwesomeHub(5, 34).Results.IsExactly(Expected);
}
