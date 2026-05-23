namespace Regression;

[TestClass, TestCategory("Regression")]
public class BetaTests : RegressionTestBase<BetaResult>
{
    public BetaTests() : base("beta.standard.json") { }

    private const int n = 50;

    [TestMethod]
    public override void Series_AgainstBaseline_MatchesExactly() => OtherQuotes.ToBeta(Quotes, n).IsExactly(Expected);

    [TestMethod]
    public override void Buffer_AgainstBaseline_MatchesExactly() => Assert.Inconclusive("Test not yet implemented");

    [TestMethod]
    public override void Stream_AgainstBaseline_MatchesExactly() => Assert.Inconclusive("Test not yet implemented");
}
