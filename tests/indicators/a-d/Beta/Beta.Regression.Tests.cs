namespace Regression;

[TestClass, TestCategory("Regression")]
public class BetaTests : RegressionTestBase<BetaResult>
{
    public BetaTests() : base("beta.standard.json") { }

    private const int n = 50;

    [TestMethod]
    public override void Series() => OtherQuotes.ToBeta(Quotes, n).IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => Assert.Inconclusive("Test not yet implemented");

    [TestMethod]
    public override void Stream() => Assert.Inconclusive("Test not yet implemented");
}
