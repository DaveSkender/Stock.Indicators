namespace Regression;

[TestClass, TestCategory("Regression")]
public class SupertrendTests : RegressionTestBase<SuperTrendResult>
{
    public SupertrendTests() : base("supertrend.standard.json") { }

    [TestMethod]
    public override void Series_AgainstBaseline_MatchesExactly() => Quotes.ToSuperTrend(10, 3).IsExactly(Expected);

    [TestMethod]
    public override void Buffer_AgainstBaseline_MatchesExactly() => Quotes.ToSuperTrendList(10, 3).IsExactly(Expected);

    [TestMethod]
    public override void Stream_AgainstBaseline_MatchesExactly() => Quotes.ToSuperTrendHub(10, 3).Results.IsExactly(Expected);
}
