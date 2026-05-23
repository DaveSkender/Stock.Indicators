namespace Regression;

[TestClass, TestCategory("Regression")]
public class StochTests : RegressionTestBase<StochResult>
{
    public StochTests() : base("stoch.standard.json") { }

    [TestMethod]
    public override void Series_AgainstBaseline_MatchesExactly() => Quotes.ToStoch(14, 3, 3).IsExactly(Expected);

    [TestMethod]
    public override void Buffer_AgainstBaseline_MatchesExactly() => Quotes.ToStochList(14, 3, 3).IsExactly(Expected);

    [TestMethod]
    public override void Stream_AgainstBaseline_MatchesExactly() => QuoteHub.ToStochHub(14, 3, 3).Results.IsExactly(Expected);
}
