namespace Regression;

[TestClass, TestCategory("Regression")]
public class CorrelationTests : RegressionTestBase<CorrResult>
{
    public CorrelationTests() : base("corr.standard.json") { }

    [TestMethod]
    public override void Series()
    {
        // Correlation requires two IReusable series - use same quotes for both sourceEval and sourceMrkt
        IReadOnlyList<IReusable> sourceEval = Quotes.Cast<IReusable>().ToList();
        IReadOnlyList<IReusable> sourceMrkt = Quotes.Cast<IReusable>().ToList();
        sourceEval.ToCorrelation(sourceMrkt, 50).AssertEquals(Expected);
    }

    [TestMethod]
    public override void Buffer() => Assert.Inconclusive("Buffer implementation not yet available");

    [TestMethod]
    public override void Stream() => Assert.Inconclusive("Stream implementation has dual-provider synchronization issues - requires architectural review");
}
