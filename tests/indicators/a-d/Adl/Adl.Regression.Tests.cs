namespace Regression;

[TestClass, TestCategory("Regression")]
public class AdlTests : RegressionTestBase<AdlResult>
{
    public AdlTests() : base("adl.standard.json") { }

    [TestMethod]
    public override void Series_AgainstBaseline_MatchesExactly() => Quotes.ToAdl().IsExactly(Expected);

    [TestMethod]
    public override void Buffer_AgainstBaseline_MatchesExactly() => Quotes.ToAdlList().IsExactly(Expected);

    [TestMethod]
    public override void Stream_AgainstBaseline_MatchesExactly() => QuoteHub.ToAdlHub().Results.IsExactly(Expected);
}
