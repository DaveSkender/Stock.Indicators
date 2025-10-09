
namespace Regression;

[TestClass, TestCategory("Regression")]
public class RenkoatrTests : RegressionTestBase<RenkoResult>
{
    public RenkoatrTests() : base("renko-atr.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToRenko(14, EndType.Close).AssertEquals(Expected);

    [TestMethod]
    public override void Buffer() => Assert.Inconclusive("Buffer implementation not yet available");

    [TestMethod]
    public override void Stream() => Assert.Inconclusive("Stream implementation not yet available");
}
