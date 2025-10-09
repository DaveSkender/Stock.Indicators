namespace Regression;

[TestClass, TestCategory("Regression")]
public class StcTests : RegressionTestBase<StcResult>
{
    public StcTests() : base("stc.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToStc(10, 23, 50).AssertEquals(Expected);

    [TestMethod]
    public override void Buffer() => Assert.Inconclusive("Buffer implementation not yet available");

    [TestMethod]
    public override void Stream() => Assert.Inconclusive("Stream implementation not yet available");
}
