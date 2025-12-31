namespace Regression;

[TestClass, TestCategory("Regression")]
public class BetaTests : RegressionTestBase<BetaResult>
{
    public BetaTests() : base("beta.standard.json") { }

    private const int n = 14;

    [TestMethod]
    public override void Series() => OtherQuotes.ToBeta(Quotes, n).IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => OtherQuotes.ToBetaList(Quotes, n).IsExactly(Expected);

    [TestMethod]
    public override void Stream() => OtherQuotes.ToBetaHub(Quotes, n).Results.IsExactly(Expected);
}
