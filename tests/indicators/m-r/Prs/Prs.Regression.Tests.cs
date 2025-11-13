namespace Regression;

[TestClass, TestCategory("Regression")]
public class PrsTests : RegressionTestBase<PrsResult>
{
    public PrsTests() : base("prs.standard.json") { }

    [TestMethod]
    public override void Series()
    {
        // Prs requires two IReusable series - use same quotes for both sourceEval and sourceMrkt
        IReadOnlyList<IReusable> sourceEval = Quotes.Cast<IReusable>().ToList();
        IReadOnlyList<IReusable> sourceMrkt = Quotes.Cast<IReusable>().ToList();
        sourceEval.ToPrs(sourceMrkt).AssertEquals(Expected);
    }

    [TestMethod]
    public override void Buffer() => Assert.Inconclusive("Buffer implementation not yet available");

    [TestMethod]
    public override void Stream() => Assert.Inconclusive("Stream implementation not yet available");
}
