namespace Regression;

[TestClass, TestCategory("Regression")]
public class StdDevTests : RegressionTestBase<StdDevResult>
{
    public StdDevTests() : base("stdev.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToStdDev().IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => Quotes.ToStdDevList(14).IsExactly(Expected);

    [TestMethod]
    public override void Stream() => Quotes.ToStdDevHub(14).Results.IsExactly(Expected);
}
