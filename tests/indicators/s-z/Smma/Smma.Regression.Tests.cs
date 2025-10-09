
namespace Regression;

[TestClass, TestCategory("Regression")]
public class SmmaTests : RegressionTestBase<SmmaResult>
{
    public SmmaTests() : base("smma.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToSmma(20).AssertEquals(Expected);

    [TestMethod]
    public override void Buffer() => Assert.Inconclusive("Buffer implementation not yet available");

    [TestMethod]
    public override void Stream() => Assert.Inconclusive("Stream implementation not yet available");
}
