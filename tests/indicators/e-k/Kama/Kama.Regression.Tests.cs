namespace Regression;

[TestClass, TestCategory("Regression")]
public class KamaTests : RegressionTestBase<KamaResult>
{
    public KamaTests() : base("kama.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToKama(10, 2, 30).IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => Assert.Inconclusive("Buffer implementation not yet available");

    [TestMethod]
    public override void Stream() => Assert.Inconclusive("Stream implementation not yet available");
}
