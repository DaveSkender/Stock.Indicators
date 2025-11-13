namespace Regression;

[TestClass, TestCategory("Regression")]
public class BetaTests : RegressionTestBase<BetaResult>
{
    public BetaTests() : base("beta.standard.json") { }

    [TestMethod]
    public override void Series()
    {
        // Beta requires two IReusable series - use same quotes for both sourceEval and sourceMrkt
        IReadOnlyList<IReusable> sourceEval = Quotes.Cast<IReusable>().ToList();
        IReadOnlyList<IReusable> sourceMrkt = Quotes.Cast<IReusable>().ToList();
        sourceEval.ToBeta(sourceMrkt, 50).AssertEquals(Expected);
    }

    [TestMethod]
    public override void Buffer()
    {
        // Beta requires two IReusable series - use same quotes for both sourceEval and sourceMrkt
        IReadOnlyList<IReusable> sourceEval = Quotes.Cast<IReusable>().ToList();
        IReadOnlyList<IReusable> sourceMrkt = Quotes.Cast<IReusable>().ToList();
        sourceEval.ToBetaList(sourceMrkt, 50).AssertEquals(Expected);
    }

    [TestMethod]
    public override void Stream() => Assert.Inconclusive("Stream implementation not yet available");
}
