namespace Regression;

[TestClass, TestCategory("Regression")]
public class ChandelierTests : RegressionTestBase<ChandelierResult>
{
    public ChandelierTests() : base("chexit.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToChandelier(22, 3).IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => Quotes.ToChandelierList(22, 3).IsExactly(Expected);

    [TestMethod]
    public override void Stream()
    {
        QuoteHub hub = new();
        hub.Add(Quotes);
        hub.ToChandelierHub(22, 3).Results.IsExactly(Expected);
        hub.EndTransmission();
    }
}
