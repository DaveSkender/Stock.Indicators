using TestsUtilities;

namespace Regression;

[TestClass, TestCategory("Regression")]
public class ObvTests : RegressionTestBase<ObvResult>
{
    public ObvTests() : base("obv.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToObv().AssertEquals(Expected);

    [TestMethod]
    public override void Buffer() => Assert.Inconclusive("Buffer implementation not yet available");

    [TestMethod]
    public override void Stream() => Assert.Inconclusive("Stream implementation not yet available");
}
