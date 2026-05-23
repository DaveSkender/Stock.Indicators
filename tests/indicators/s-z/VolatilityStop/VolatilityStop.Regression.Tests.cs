namespace Regression;

[TestClass, TestCategory("Regression")]
public class VolatilitystopTests : RegressionTestBase<VolatilityStopResult>
{
    public VolatilitystopTests() : base("vol-stop.standard.json") { }

    [TestMethod]
    public override void Series_AgainstBaseline_MatchesExactly() => Quotes.ToVolatilityStop().IsExactly(Expected);

    [TestMethod]
    public override void Buffer_AgainstBaseline_MatchesExactly() => Quotes.ToVolatilityStopList().IsExactly(Expected);

    [TestMethod]
    public override void Stream_AgainstBaseline_MatchesExactly() => QuoteHub.ToVolatilityStopHub().Results.IsExactly(Expected);
}
