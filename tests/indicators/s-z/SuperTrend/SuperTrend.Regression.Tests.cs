
namespace Regression;

[TestClass, TestCategory("Regression")]
public class SupertrendTests : RegressionTestBase<SuperTrendResult>
{
    public SupertrendTests() : base("supertrend.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToSuperTrend(10, 3).AssertEquals(Expected);

    [TestMethod]
    public override void Buffer() => Assert.Inconclusive("Buffer implementation not yet available");

    [TestMethod]
    public override void Stream() => Assert.Inconclusive("Stream implementation not yet available");
}
