
namespace Regression;

[TestClass, TestCategory("Regression")]
public class AtrstopTests : RegressionTestBase<AtrStopResult>
{
    public AtrstopTests() : base("atr-stop.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToAtrStop(21, 3).AssertEquals(Expected);

    [TestMethod]
    public override void Buffer() => Assert.Inconclusive("Buffer implementation not yet available");

    [TestMethod]
    public override void Stream() => Assert.Inconclusive("Stream implementation not yet available");
}
