namespace Regression;

[TestClass, TestCategory("Regression")]
public class StdDevTests : RegressionTestBase<StdDevResult>
{
    public StdDevTests() : base("stdev.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToStdDev().AssertEquals(Expected);

    [TestMethod]
    public override void Buffer() => Quotes.ToStdDevList(14).AssertEquals(Expected);

    [TestMethod]
    public override void Stream() => Quotes.ToStdDevHub(14).Results.AssertEquals(Expected);
}
