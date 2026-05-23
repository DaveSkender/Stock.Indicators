namespace Regression;

[TestClass, TestCategory("Regression")]
public class ObvTests : RegressionTestBase<ObvResult>
{
    public ObvTests() : base("obv.standard.json") { }

    [TestMethod]
    public override void Series_AgainstBaseline_MatchesExactly() => Quotes.ToObv().IsExactly(Expected);

    [TestMethod]
    public override void Buffer_AgainstBaseline_MatchesExactly() => Quotes.ToObvList().IsExactly(Expected);

    [TestMethod]
    public override void Stream_AgainstBaseline_MatchesExactly() => QuoteHub.ToObvHub().Results.IsExactly(Expected);
}
