namespace Regression;

[TestClass, TestCategory("Regression")]
public class MamaTests : RegressionTestBase<MamaResult>
{
    public MamaTests() : base("mama.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToMama(0.5, 0.05).AssertEquals(Expected);

    [TestMethod]
    public override void Buffer() => Assert.Inconclusive("Buffer implementation not yet available");

    [TestMethod]
    public override void Stream() => Assert.Inconclusive("Stream implementation not yet available");
}
