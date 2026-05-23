namespace Regression;

[TestClass, TestCategory("Regression")]
public class StcTests : RegressionTestBase<StcResult>
{
    public StcTests() : base("stc.standard.json") { }

    [TestMethod]
    public override void Series_AgainstBaseline_MatchesExactly() => Quotes.ToStc(10, 23, 50).IsExactly(Expected);

    [TestMethod]
    public override void Buffer_AgainstBaseline_MatchesExactly() => Quotes.ToStcList(10, 23, 50).IsExactly(Expected);

    [TestMethod]
    public override void Stream_AgainstBaseline_MatchesExactly()
    {
        QuoteHub hub = new();
        hub.Add(Quotes);
        hub.ToStcHub(10, 23, 50).Results.IsExactly(Expected);
    }
}
