namespace Regression;

[TestClass, TestCategory("Regression")]
public class RollingPivotsTests : RegressionTestBase<RollingPivotsResult>
{
    public RollingPivotsTests() : base("rolling-pivots.standard.json") { }

    [TestMethod]
    public override void Series_AgainstBaseline_MatchesExactly() => Quotes.ToRollingPivots().IsExactly(Expected);

    [TestMethod]
    public override void Buffer_AgainstBaseline_MatchesExactly() => Quotes.ToRollingPivotsList().IsExactly(Expected);

    [TestMethod]
    public override void Stream_AgainstBaseline_MatchesExactly() => QuoteHub.ToRollingPivotsHub().Results.IsExactly(Expected);
}
