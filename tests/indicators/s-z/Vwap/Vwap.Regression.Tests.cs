namespace Regression;

[TestClass, TestCategory("Regression")]
public class VwapTests : RegressionTestBase<VwapResult>
{
    public VwapTests() : base("vwap.standard.json") { }

    [TestMethod]
    public override void Series_AgainstBaseline_MatchesExactly() => Quotes.ToVwap().IsExactly(Expected);

    [TestMethod]
    public override void Buffer_AgainstBaseline_MatchesExactly() => Quotes.ToVwapList().IsExactly(Expected);

    [TestMethod]
    public override void Stream_AgainstBaseline_MatchesExactly() => Quotes.ToVwapHub().Results.IsExactly(Expected);
}
