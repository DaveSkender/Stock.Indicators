namespace Regression;

[TestClass, TestCategory("Regression")]
public class VwmaTests : RegressionTestBase<VwmaResult>
{
    public VwmaTests() : base("vwma.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToVwma(14).IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => Quotes.ToVwmaList(14).IsExactly(Expected);

    [TestMethod]
    public override void Stream() => QuoteHub.ToVwmaHub(14).Results.IsExactly(Expected);
}
