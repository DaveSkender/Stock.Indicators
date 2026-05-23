namespace Regression;

[TestClass, TestCategory("Regression")]
public class ChandelierTests : RegressionTestBase<ChandelierResult>
{
    public ChandelierTests() : base("chexit.standard.json") { }

    [TestMethod]
    public override void Series_AgainstBaseline_MatchesExactly() => Quotes.ToChandelier(22, 3).IsExactly(Expected);

    [TestMethod]
    public override void Buffer_AgainstBaseline_MatchesExactly() => Quotes.ToChandelierList(22, 3).IsExactly(Expected);

    [TestMethod]
    public override void Stream_AgainstBaseline_MatchesExactly()
    {
        QuoteHub hub = new();
        hub.Add(Quotes);
        hub.ToChandelierHub(22, 3).Results.IsExactly(Expected);
        hub.EndTransmission();
    }
}
