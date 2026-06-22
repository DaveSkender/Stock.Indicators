namespace Regression;

[TestClass, TestCategory("Regression")]
public class ChopTests : RegressionTestBase<ChopResult>
{
    public ChopTests() : base("chop.standard.json") { }

    [TestMethod]
    public override void Series_AgainstBaseline_MatchesExactly() => Bars.ToChop(14).IsExactly(Expected);

    [TestMethod]
    public override void Buffer_AgainstBaseline_MatchesExactly() => Bars.ToChopList(14).IsExactly(Expected);

    [TestMethod]
    public override void Stream_AgainstBaseline_MatchesExactly() => BarHub.ToChopHub(14).Results.IsExactly(Expected);
}
