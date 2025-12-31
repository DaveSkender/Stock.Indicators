namespace Regression;

[TestClass, TestCategory("Regression")]
public class PrsTests : RegressionTestBase<PrsResult>
{
    public PrsTests() : base("prs.standard.json") { }

    private const int n = 14;

    [TestMethod]
    public override void Series() => OtherQuotes.ToPrs(Quotes, n).IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => OtherQuotes.ToPrsList(Quotes, n).IsExactly(Expected);

    [TestMethod]
    public override void Stream() => OtherQuotes.ToPrsHub(Quotes, n).Results.IsExactly(Expected);
}
