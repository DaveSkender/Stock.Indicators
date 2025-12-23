namespace Regression;

[TestClass, TestCategory("Regression")]
public class MamaTests : RegressionTestBase<MamaResult>
{
    public MamaTests() : base("mama.standard.json") { }

    [TestMethod]
    public override void Series() =>
        // MAMA uses recursive MESA adaptive calculations that accumulate
        // floating-point precision differences at ~14-15th decimal place
        Quotes.ToMama(0.5, 0.05).IsApproximately(Expected, precision: 13);

    [TestMethod]
    public override void Buffer() => Assert.Inconclusive("Buffer implementation not yet available");

    [TestMethod]
    public override void Stream() => Assert.Inconclusive("Stream implementation not yet available");
}
