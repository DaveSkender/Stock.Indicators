namespace Regression;

[TestClass, TestCategory("Regression")]
public class KvoTests : RegressionTestBase<KvoResult>
{
    public KvoTests() : base("kvo.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToKvo(34, 55, 13).IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => Quotes.ToKvoList(34, 55, 13).IsExactly(Expected);

    [TestMethod]
    public override void Stream()
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
