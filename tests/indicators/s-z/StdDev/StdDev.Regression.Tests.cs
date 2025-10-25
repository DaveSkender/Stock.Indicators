namespace Regression;

[TestClass, TestCategory("Regression")]
public class StdDevTests : RegressionTestBase<StdDevResult>
{
    public StdDevTests() : base("stdev.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToStdDev().AssertEquals(Expected);

    [TestMethod]
    public override void Buffer() => Assert.Inconclusive("Buffer implementation not yet available");

    [TestMethod]
    public override void Stream() => Assert.Inconclusive("Stream implementation not yet available");
}
