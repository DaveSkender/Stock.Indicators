namespace Regression;

[TestClass, TestCategory("Regression")]
public class KvoTests : RegressionTestBase<KvoResult>
{
    public KvoTests() : base("kvo.standard.json") { }

    [TestMethod]
    public override void Series_AgainstBaseline_MatchesExactly() => Quotes.ToKvo(34, 55, 13).IsExactly(Expected);

    [TestMethod]
    public override void Buffer_AgainstBaseline_MatchesExactly() => Quotes.ToKvoList(34, 55, 13).IsExactly(Expected);

    [TestMethod]
    public override void Stream_AgainstBaseline_MatchesExactly()
    {
        QuoteHub quoteHub = new();
        KvoHub hub = quoteHub.ToKvoHub(34, 55, 13);

        foreach (Quote q in Quotes)
        {
            quoteHub.Add(q);
        }

        hub.Results.IsExactly(Expected);

        hub.Unsubscribe();
        quoteHub.EndTransmission();
    }
}
