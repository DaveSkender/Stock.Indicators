namespace Regression;

[TestClass, TestCategory("Regression")]
public class SmmaTests : RegressionTestBase<SmmaResult>
{
    public SmmaTests() : base("smma.standard.json") { }

    [TestMethod]
    public override void Series_AgainstBaseline_MatchesExactly() => Quotes.ToSmma(20).IsExactly(Expected);

    [TestMethod]
    public override void Buffer_AgainstBaseline_MatchesExactly() => Quotes.ToSmmaList(20).IsExactly(Expected);

    [TestMethod]
    public override void Stream_AgainstBaseline_MatchesExactly() => QuoteHub.ToSmmaHub(20).Results.IsExactly(Expected);
}
