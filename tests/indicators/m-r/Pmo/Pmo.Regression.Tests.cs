using TestsUtilities;

namespace Regression;

[TestClass, TestCategory("Regression")]
public class PmoTests : RegressionTestBase<PmoResult>
{
    public PmoTests() : base("pmo.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToPmo(35, 20, 10).AssertEquals(Expected);

    [TestMethod]
    public override void Buffer() => Assert.Inconclusive("Buffer implementation not yet available");

    [TestMethod]
    public override void Stream() => Assert.Inconclusive("Stream implementation not yet available");
}
