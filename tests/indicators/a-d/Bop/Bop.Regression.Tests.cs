namespace Regression;

[TestClass, TestCategory("Regression")]
public class BopTests : RegressionTestBase<BopResult>
{
    public BopTests() : base("bop.standard.json") { }

    [TestMethod]
    public override void Series_AgainstBaseline_MatchesExactly() => Bars.ToBop().IsExactly(Expected);

    [TestMethod]
    public override void Buffer_AgainstBaseline_MatchesExactly() => Bars.ToBopList(14).IsExactly(Expected);

    [TestMethod]
    public override void Stream_AgainstBaseline_MatchesExactly() => BarHub.ToBopHub(14).Results.IsExactly(Expected);
}
