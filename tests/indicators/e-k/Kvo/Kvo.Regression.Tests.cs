
namespace Regression;

[TestClass, TestCategory("Regression")]
public class KvoTests : RegressionTestBase<KvoResult>
{
    public KvoTests() : base("kvo.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToKvo(34, 55, 13).AssertEquals(Expected);

    [TestMethod]
    public override void Buffer() => Assert.Inconclusive("Buffer implementation not yet available");

    [TestMethod]
    public override void Stream() => Assert.Inconclusive("Stream implementation not yet available");
}
