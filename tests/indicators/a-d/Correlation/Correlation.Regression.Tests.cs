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
        sourceEval.ToCorrelation(sourceMrkt, 50).IsExactly(Expected);
    }

    [TestMethod]
    public override void Buffer()
    {
        IReadOnlyList<IReusable> sourceEval = Quotes.Cast<IReusable>().ToList();
        IReadOnlyList<IReusable> sourceMrkt = Quotes.Cast<IReusable>().ToList();
        sourceEval.ToCorrelationList(sourceMrkt, 50).IsExactly(Expected);
    }

    [TestMethod]
    public override void Stream()
    {
        // Create two separate quote hubs for dual-provider pattern
        QuoteHub quoteHubEval = new();
        QuoteHub quoteHubMrkt = new();

        quoteHubEval.Add(Quotes);
        quoteHubMrkt.Add(Quotes);

        quoteHubEval.ToCorrelationHub(quoteHubMrkt, 50).Results.IsExactly(Expected);
    }
}
