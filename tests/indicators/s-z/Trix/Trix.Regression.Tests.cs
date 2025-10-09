using TestsUtilities;

namespace Regression;

[TestClass, TestCategory("Regression")]
public class TrixTests : RegressionTestBase<TrixResult>
{
    public TrixTests() : base("trix.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToTrix(20).AssertEquals(Expected);

    [TestMethod]
    public override void Buffer() => Assert.Inconclusive("Buffer implementation not yet available");

    [TestMethod]
    public override void Stream() => Assert.Inconclusive("Stream implementation not yet available");
}
