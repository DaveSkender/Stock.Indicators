namespace Regression;

[TestClass, TestCategory("Regression")]
public class VortexTests : RegressionTestBase<VortexResult>
{
    public VortexTests() : base("vortex.standard.json") { }

    [TestMethod]
    public override void Series_AgainstBaseline_MatchesExactly() => Quotes.ToVortex(14).IsExactly(Expected);

    [TestMethod]
    public override void Buffer_AgainstBaseline_MatchesExactly() => Quotes.ToVortexList(14).IsExactly(Expected);

    [TestMethod]
    public override void Stream_AgainstBaseline_MatchesExactly() => QuoteHub.ToVortexHub(14).Results.IsExactly(Expected);
}
