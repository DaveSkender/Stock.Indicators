namespace Regression;

[TestClass, TestCategory("Regression")]
public class HeikinashiTests : RegressionTestBase<HeikinAshiResult>
{
    public HeikinashiTests() : base("heikinashi.standard.json") { }

    [TestMethod]
    public override void Series_AgainstBaseline_MatchesExactly() => Quotes.ToHeikinAshi().IsExactly(Expected);

    [TestMethod]
    public override void Buffer_AgainstBaseline_MatchesExactly() => Quotes.ToHeikinAshiList().IsExactly(Expected);

    [TestMethod]
    public override void Stream_AgainstBaseline_MatchesExactly() => Quotes.ToHeikinAshiHub().Results.IsExactly(Expected);
}
