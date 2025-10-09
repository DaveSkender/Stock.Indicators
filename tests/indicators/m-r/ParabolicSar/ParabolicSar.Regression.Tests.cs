
namespace Regression;

[TestClass, TestCategory("Regression")]
public class ParabolicsarTests : RegressionTestBase<ParabolicSarResult>
{
    public ParabolicsarTests() : base("psar.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToParabolicSar(0.02, 0.2).AssertEquals(Expected);

    [TestMethod]
    public override void Buffer() => Assert.Inconclusive("Buffer implementation not yet available");

    [TestMethod]
    public override void Stream() => Assert.Inconclusive("Stream implementation not yet available");
}
