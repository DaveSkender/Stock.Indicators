
namespace Regression;

[TestClass, TestCategory("Regression")]
public class SlopeTests : RegressionTestBase<SlopeResult>
{
    public SlopeTests() : base("slope.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToSlope(20).AssertEquals(Expected);

    [TestMethod]
    public override void Buffer() => Assert.Inconclusive("Buffer implementation not yet available");

    [TestMethod]
    public override void Stream() => Assert.Inconclusive("Stream implementation not yet available");
}
