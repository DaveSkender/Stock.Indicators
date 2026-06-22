namespace Regression;

[TestClass, TestCategory("Regression")]
public class AdlTests : RegressionTestBase<AdlResult>
{
    public AdlTests() : base("adl.standard.json") { }

    [TestMethod]
    public override void Series_AgainstBaseline_MatchesExactly() => Bars.ToAdl().IsExactly(Expected);

    [TestMethod]
    public override void Buffer_AgainstBaseline_MatchesExactly() => Bars.ToAdlList().IsExactly(Expected);

    [TestMethod]
    public override void Stream_AgainstBaseline_MatchesExactly() => BarHub.ToAdlHub().Results.IsExactly(Expected);
}
