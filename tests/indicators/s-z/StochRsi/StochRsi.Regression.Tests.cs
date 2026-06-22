namespace Regression;

[TestClass, TestCategory("Regression")]
public class StochrsiTests : RegressionTestBase<StochRsiResult>
{
    public StochrsiTests() : base("stoch-rsi.standard.json") { }

    [TestMethod]
    public override void Series_AgainstBaseline_MatchesExactly() => Bars.ToStochRsi().IsExactly(Expected);

    [TestMethod]
    public override void Buffer_AgainstBaseline_MatchesExactly() => Bars.ToStochRsiList(14, 14, 3, 1).IsExactly(Expected);

    [TestMethod]
    public override void Stream_AgainstBaseline_MatchesExactly() => BarHub.ToStochRsiHub().Results.IsExactly(Expected);
}
