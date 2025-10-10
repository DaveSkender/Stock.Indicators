namespace Regression;

[TestClass, TestCategory("Regression")]
public class MamaTests : RegressionTestBase<MamaResult>
{
    // Use DoublePrecision from TestBase for floating-point differences across CPU architectures
    private const double Precision = 1E-13;

    public MamaTests() : base("mama.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToMama(0.5, 0.05).AssertEquals(Expected, Precision);

    [TestMethod]
    public override void Buffer() => Assert.Inconclusive("Buffer implementation not yet available");

    [TestMethod]
    public override void Stream() => Assert.Inconclusive("Stream implementation not yet available");
}
