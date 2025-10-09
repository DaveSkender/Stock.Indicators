
namespace Regression;

[TestClass, TestCategory("Regression")]
public class CciTests : RegressionTestBase<CciResult>
{
    public CciTests() : base("cci.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToCci(20).AssertEquals(Expected);

    [TestMethod]
    public override void Buffer() => Assert.Inconclusive("Buffer implementation not yet available");

    [TestMethod]
    public override void Stream() => Assert.Inconclusive("Stream implementation not yet available");
}
