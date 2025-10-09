using TestsUtilities;

namespace Regression;

[TestClass, TestCategory("Regression")]
public class ElderrayTests : RegressionTestBase<ElderRayResult>
{
    public ElderrayTests() : base("elder-ray.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToElderRay(13).AssertEquals(Expected);

    [TestMethod]
    public override void Buffer() => Assert.Inconclusive("Buffer implementation not yet available");

    [TestMethod]
    public override void Stream() => Assert.Inconclusive("Stream implementation not yet available");
}
