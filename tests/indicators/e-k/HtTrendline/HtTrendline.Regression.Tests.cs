namespace Regression;

[TestClass, TestCategory("Regression")]
public class HtTrendlineTests : RegressionTestBase<HtlResult>
{
    public HtTrendlineTests() : base("htl.standard.json") { }

    [TestMethod]
    public override void Series_AgainstBaseline_MatchesExactly() => Bars.ToHtTrendline().IsExactly(Expected);

    [TestMethod]
    public override void Buffer_AgainstBaseline_MatchesExactly() => Bars.ToHtTrendlineList().IsExactly(Expected);

    [TestMethod]
    public override void Stream_AgainstBaseline_MatchesExactly() => Bars.ToHtTrendlineHub().Results.IsExactly(Expected);
}
