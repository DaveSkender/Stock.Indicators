namespace Regression;

[TestClass, TestCategory("Regression")]
public class AdxTests : RegressionTestBase<AdxResult>
{
    public AdxTests() : base("adx.standard.json") { }

    [TestMethod]
    public override void Series_AgainstBaseline_MatchesExactly() => Bars.ToAdx(14).IsExactly(Expected);

    [TestMethod]
    public override void Buffer_AgainstBaseline_MatchesExactly() => Bars.ToAdxList(14).IsExactly(Expected);

    [TestMethod]
    public override void Stream_AgainstBaseline_MatchesExactly() => BarHub.ToAdxHub(14).Results.IsExactly(Expected);
}
