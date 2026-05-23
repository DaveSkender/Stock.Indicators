namespace Regression;

[TestClass, TestCategory("Regression")]
public class TsiTests : RegressionTestBase<TsiResult>
{
    public TsiTests() : base("tsi.standard.json") { }

    [TestMethod]
    public override void Series_AgainstBaseline_MatchesExactly() => Quotes.ToTsi(25, 13, 7).IsExactly(Expected);

    [TestMethod]
    public override void Buffer_AgainstBaseline_MatchesExactly() => Quotes.ToTsiList(25, 13, 7).IsExactly(Expected);

    [TestMethod]
    public override void Stream_AgainstBaseline_MatchesExactly() => QuoteHub.ToTsiHub(25, 13, 7).Results.IsExactly(Expected);
}
