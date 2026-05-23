namespace Regression;

[TestClass, TestCategory("Regression")]
public class StarcBandsTests : RegressionTestBase<StarcBandsResult>
{
    public StarcBandsTests() : base("starc.standard.json") { }

    [TestMethod]
    public override void Series_AgainstBaseline_MatchesExactly() => Quotes.ToStarcBands().IsExactly(Expected);

    [TestMethod]
    public override void Buffer_AgainstBaseline_MatchesExactly() => Quotes.ToStarcBandsList().IsExactly(Expected);

    [TestMethod]
    public override void Stream_AgainstBaseline_MatchesExactly() => QuoteHub.ToStarcBandsHub().Results.IsExactly(Expected);
}
