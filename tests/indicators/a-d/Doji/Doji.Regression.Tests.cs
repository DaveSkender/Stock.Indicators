
namespace Regression;

[TestClass, TestCategory("Regression")]
public class DojiTests : RegressionTestBase<CandleResult>
{
    public DojiTests() : base("doji.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToDoji(0.1).AssertEquals(Expected);

    [TestMethod]
    public override void Buffer() => Assert.Inconclusive("Buffer implementation not yet available");

    [TestMethod]
    public override void Stream() => Assert.Inconclusive("Stream implementation not yet available");
}
