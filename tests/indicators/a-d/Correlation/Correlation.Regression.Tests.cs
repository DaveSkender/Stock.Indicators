namespace Regression;

[TestClass, TestCategory("Regression")]
public class CorrelationTests : RegressionTestBase<CorrResult>
{
    public CorrelationTests() : base("corr.standard.json") { }

    private const int n = 20;

    [TestMethod]
    public override void Series() => OtherQuotes.ToCorrelation(Quotes, n).IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => Assert.Inconclusive("Test not yet implemented");

    [TestMethod]
    public override void Stream() => Assert.Inconclusive("Test not yet implemented");
}
