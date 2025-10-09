
namespace Regression;

[TestClass, TestCategory("Regression")]
public class WmaTests : RegressionTestBase<WmaResult>
{
    public WmaTests() : base("wma.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToWma(14).AssertEquals(Expected);

    [TestMethod]
    public override void Buffer() => Assert.Inconclusive("Buffer implementation not yet available");

    [TestMethod]
    public override void Stream() => quoteHub.ToWmaHub(14).Results.AssertEquals(Expected);
}
