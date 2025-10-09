
namespace Regression;

[TestClass, TestCategory("Regression")]
public class DonchianTests : RegressionTestBase<DonchianResult>
{
    public DonchianTests() : base("donchian.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToDonchian(20).AssertEquals(Expected);

    [TestMethod]
    public override void Buffer() => Assert.Inconclusive("Buffer implementation not yet available");

    [TestMethod]
    public override void Stream() => Assert.Inconclusive("Stream implementation not yet available");
}
