namespace Regression;

[TestClass, TestCategory("Regression")]
public class MaenvelopesTests : RegressionTestBase<MaEnvelopeResult>
{
    public MaenvelopesTests() : base("ma-env.standard.json") { }

    [TestMethod]
    public override void Series_AgainstBaseline_MatchesExactly() => Quotes.ToMaEnvelopes(20, 2.5).IsExactly(Expected);

    [TestMethod]
    public override void Buffer_AgainstBaseline_MatchesExactly() => Quotes.ToMaEnvelopesList(20, 2.5).IsExactly(Expected);

    [TestMethod]
    public override void Stream_AgainstBaseline_MatchesExactly() => QuoteHub.ToMaEnvelopesHub(20, 2.5).Results.IsExactly(Expected);
}
