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
        sourceEval.ToPrs(sourceMrkt).IsExactly(Expected);
    }

    [TestMethod]
    public override void Buffer()
    {
        IReadOnlyList<IReusable> sourceEval = Quotes.Cast<IReusable>().ToList();
        IReadOnlyList<IReusable> sourceMrkt = Quotes.Cast<IReusable>().ToList();
        sourceEval.ToPrsList(sourceMrkt).IsExactly(Expected);
    }

    [TestMethod]
    public override void Stream()
    {
        // Create two separate quote hubs for dual-provider pattern
        QuoteHub quoteHubEval = new();
        QuoteHub quoteHubMrkt = new();

        quoteHubEval.Add(Quotes);
        quoteHubMrkt.Add(Quotes);

        quoteHubEval.ToPrsHub(quoteHubMrkt).Results.IsExactly(Expected);
    }
}
