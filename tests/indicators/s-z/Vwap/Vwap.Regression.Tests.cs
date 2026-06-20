namespace Regression;

[TestClass, TestCategory("Regression")]
public class VwapTests : RegressionTestBase<VwapResult>
{
    public VwapTests() : base("vwap.standard.json") { }

    [TestMethod]
    public override void Series_AgainstBaseline_MatchesExactly() => Bars.ToVwap().IsExactly(Expected);

    [TestMethod]
    public override void Buffer_AgainstBaseline_MatchesExactly() => Bars.ToVwapList().IsExactly(Expected);

    [TestMethod]
    public override void Stream_AgainstBaseline_MatchesExactly() => Bars.ToVwapHub().Results.IsExactly(Expected);
}
