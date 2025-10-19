namespace Regression;

[TestClass, TestCategory("Regression")]
public class ChandelierTests : RegressionTestBase<ChandelierResult>
{
    public ChandelierTests() : base("chexit.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToChandelier(22, 3).AssertEquals(Expected);

    [TestMethod]
    public override void Buffer() => Assert.Inconclusive("Buffer implementation not yet available");

    [TestMethod]
    public override void Stream()
    {
        QuoteHub hub = new();
        hub.Add(Quotes);
        hub.ToChandelierHub(22, 3).Results.AssertEquals(Expected);
        hub.EndTransmission();
    }
}
