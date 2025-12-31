namespace Regression;

[TestClass, TestCategory("Regression")]
public class CorrelationTests : RegressionTestBase<CorrResult>
{
    public CorrelationTests() : base("corr.standard.json") { }

    private const int n = 14;

    [TestMethod]
    public override void Series() => Quotes.ToCorrelation(OtherQuotes, n).IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => Quotes.ToCorrelationList(OtherQuotes, n).IsExactly(Expected);

    [TestMethod]
    public override void Stream() => Quotes.ToCorrelationHub(OtherQuotes, n).Results.IsExactly(Expected);
}
