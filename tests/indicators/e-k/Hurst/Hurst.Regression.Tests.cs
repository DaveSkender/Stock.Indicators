namespace Regression;

[TestClass, TestCategory("Regression")]
public class HurstTests : RegressionTestBase<HurstResult>
{
    // 16th decimal digit precision tolerance for floating-point differences across CPU architectures
    private const double Precision = 1E-15;

    public HurstTests() : base("hurst.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToHurst().AssertEquals(Expected, Precision);

    [TestMethod]
    public override void Buffer() => Assert.Inconclusive("Buffer implementation not yet available");

    [TestMethod]
    public override void Stream() => Assert.Inconclusive("Stream implementation not yet available");
}
