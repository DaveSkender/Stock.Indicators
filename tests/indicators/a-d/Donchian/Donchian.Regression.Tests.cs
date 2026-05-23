namespace Regression;

[TestClass, TestCategory("Regression")]
public class DonchianTests : RegressionTestBase<DonchianResult>
{
    public DonchianTests() : base("donchian.standard.json") { }

    [TestMethod]
    public override void Series_AgainstBaseline_MatchesExactly() => Quotes.ToDonchian(20).IsExactly(Expected);

    [TestMethod]
    public override void Buffer_AgainstBaseline_MatchesExactly() => Quotes.ToDonchianList(20).IsExactly(Expected);

    [TestMethod]
    public override void Stream_AgainstBaseline_MatchesExactly() => QuoteHub.ToDonchianHub(20).Results.IsExactly(Expected);
}
