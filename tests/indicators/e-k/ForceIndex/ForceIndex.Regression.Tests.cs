
namespace Regression;

[TestClass, TestCategory("Regression")]
public class ForceindexTests : RegressionTestBase<ForceIndexResult>
{
    public ForceindexTests() : base("force.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToForceIndex(13).AssertEquals(Expected);

    [TestMethod]
    public override void Buffer() => Assert.Inconclusive("Buffer implementation not yet available");

    [TestMethod]
    public override void Stream() => Assert.Inconclusive("Stream implementation not yet available");
}
