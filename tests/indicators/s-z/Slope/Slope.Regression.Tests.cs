namespace Regression;

[TestClass, TestCategory("Regression")]
public class SlopeTests : RegressionTestBase<SlopeResult>
{
    public SlopeTests() : base("slope.standard.json") { }

    private const int n = 14;

    [TestMethod]
    public override void Series_AgainstBaseline_MatchesExactly() => Bars.ToSlope(n).IsExactly(Expected);

    [TestMethod]
    public override void Buffer_AgainstBaseline_MatchesExactly() => Bars.ToSlopeList(n).IsExactly(Expected);

    [TestMethod]
    public override void Stream_AgainstBaseline_MatchesExactly() => Bars.ToSlopeHub(n).Results.IsExactly(Expected);
}
