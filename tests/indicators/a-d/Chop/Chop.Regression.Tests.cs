namespace Regression;

[TestClass, TestCategory("Regression")]
public class ChopTests : RegressionTestBase<ChopResult>
{
    public ChopTests() : base("chop.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToChop(14).AssertEquals(Expected);

    [TestMethod]
    public override void Buffer() => Assert.Inconclusive("Buffer implementation not yet available");

    [TestMethod]
    public override void Stream() => Assert.Inconclusive("Stream implementation not yet available");
}
