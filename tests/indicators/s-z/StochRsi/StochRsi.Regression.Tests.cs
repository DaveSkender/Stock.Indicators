using TestsUtilities;

namespace Regression;

[TestClass, TestCategory("Regression")]
public class StochrsiTests : RegressionTestBase<StochRsiResult>
{
    public StochrsiTests() : base("stoch-rsi.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToStochRsi(14, 14, 3, 3).AssertEquals(Expected);

    [TestMethod]
    public override void Buffer() => Assert.Inconclusive("Buffer implementation not yet available");

    [TestMethod]
    public override void Stream() => quoteHub.ToStochRsiHub(14, 14, 3, 3).Results.AssertEquals(Expected);
}
