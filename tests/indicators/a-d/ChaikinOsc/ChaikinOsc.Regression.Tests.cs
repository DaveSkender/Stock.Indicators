
namespace Regression;

[TestClass, TestCategory("Regression")]
public class ChaikinoscTests : RegressionTestBase<ChaikinOscResult>
{
    public ChaikinoscTests() : base("chaikin-osc.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToChaikinOsc(3, 10).AssertEquals(Expected);

    [TestMethod]
    public override void Buffer() => Assert.Inconclusive("Buffer implementation not yet available");

    [TestMethod]
    public override void Stream() => Assert.Inconclusive("Stream implementation not yet available");
}
