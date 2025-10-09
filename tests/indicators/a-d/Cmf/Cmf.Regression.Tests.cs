
namespace Regression;

[TestClass, TestCategory("Regression")]
public class CmfTests : RegressionTestBase<CmfResult>
{
    public CmfTests() : base("cmf.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToCmf(20).AssertEquals(Expected);

    [TestMethod]
    public override void Buffer() => Assert.Inconclusive("Buffer implementation not yet available");

    [TestMethod]
    public override void Stream() => Assert.Inconclusive("Stream implementation not yet available");
}
