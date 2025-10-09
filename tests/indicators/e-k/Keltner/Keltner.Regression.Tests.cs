using TestsUtilities;

namespace Regression;

[TestClass, TestCategory("Regression")]
public class KeltnerTests : RegressionTestBase<KeltnerResult>
{
    public KeltnerTests() : base("keltner.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToKeltner(20, 2, 10).AssertEquals(Expected);

    [TestMethod]
    public override void Buffer() => Assert.Inconclusive("Buffer implementation not yet available");

    [TestMethod]
    public override void Stream() => Assert.Inconclusive("Stream implementation not yet available");
}
