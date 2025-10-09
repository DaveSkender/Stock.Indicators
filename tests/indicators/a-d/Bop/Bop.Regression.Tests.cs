
namespace Regression;

[TestClass, TestCategory("Regression")]
public class BopTests : RegressionTestBase<BopResult>
{
    public BopTests() : base("bop.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToBop().AssertEquals(Expected);

    [TestMethod]
    public override void Buffer() => Assert.Inconclusive("Buffer implementation not yet available");

    [TestMethod]
    public override void Stream() => Assert.Inconclusive("Stream implementation not yet available");
}
