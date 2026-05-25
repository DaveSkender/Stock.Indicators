namespace Regression;

[TestClass, TestCategory("Regression")]
public class VwmaTests : RegressionTestBase<VwmaResult>
{
    public VwmaTests() : base("vwma.standard.json") { }

    [TestMethod]
    public override void Series_AgainstBaseline_MatchesExactly() => Quotes.ToVwma(14).IsExactly(Expected);

    [TestMethod]
    public override void Buffer_AgainstBaseline_MatchesExactly() => Quotes.ToVwmaList(14).IsExactly(Expected);

    [TestMethod]
    public override void Stream_AgainstBaseline_MatchesExactly() => QuoteHub.ToVwmaHub(14).Results.IsExactly(Expected);
}
