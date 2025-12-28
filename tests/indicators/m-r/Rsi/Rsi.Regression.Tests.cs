namespace Regression;

[TestClass, TestCategory("Regression")]
public class RsiTests : RegressionTestBase<RsiResult>
{
    public RsiTests() : base("rsi.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToRsi(14).IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => Quotes.ToRsiList(14).IsExactly(Expected);

    [TestMethod]
    public override void Stream() => QuoteHub.ToRsiHub(14).Results.IsExactly(Expected);
}
