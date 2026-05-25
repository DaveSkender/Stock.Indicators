namespace Regression;

[TestClass, TestCategory("Regression")]
public class WmaTests : RegressionTestBase<WmaResult>
{
    public WmaTests() : base("wma.standard.json") { }

    [TestMethod]
    public override void Series_AgainstBaseline_MatchesExactly() => Quotes.ToWma(14).IsExactly(Expected);

    [TestMethod]
    public override void Buffer_AgainstBaseline_MatchesExactly() => Quotes.ToWmaList(14).IsExactly(Expected);

    [TestMethod]
    public override void Stream_AgainstBaseline_MatchesExactly() => QuoteHub.ToWmaHub(14).Results.IsExactly(Expected);
}
