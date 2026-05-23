namespace Regression;

[TestClass, TestCategory("Regression")]
public class ConnorsRsiTests : RegressionTestBase<ConnorsRsiResult>
{
    public ConnorsRsiTests() : base("crsi.standard.json") { }

    [TestMethod]
    public override void Series_AgainstBaseline_MatchesExactly() => Quotes.ToConnorsRsi(3, 2, 100).IsExactly(Expected);

    [TestMethod]
    public override void Buffer_AgainstBaseline_MatchesExactly() => Quotes.ToConnorsRsiList(3, 2, 100).IsExactly(Expected);

    [TestMethod]
    public override void Stream_AgainstBaseline_MatchesExactly() => QuoteHub.ToConnorsRsiHub(3, 2, 100).Results.IsExactly(Expected);
}
