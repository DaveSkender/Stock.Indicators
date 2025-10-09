
namespace Regression;

[TestClass, TestCategory("Regression")]
public class DpoTests : RegressionTestBase<DpoResult>
{
    public DpoTests() : base("dpo.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToDpo(20).AssertEquals(Expected);

    [TestMethod]
    public override void Buffer() => Assert.Inconclusive("Buffer implementation not yet available");

    [TestMethod]
    public override void Stream() => Assert.Inconclusive("Stream implementation not yet available");
}
