namespace Regression;

[TestClass, TestCategory("Regression")]
public class AwesomeTests : RegressionTestBase<AwesomeResult>
{
    public AwesomeTests() : base("awesome.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToAwesome(5, 34).AssertEquals(Expected);

    [TestMethod]
    public override void Buffer() => Assert.Inconclusive("Buffer implementation not yet available");

    [TestMethod]
    public override void Stream() => quoteHub.ToAwesomeHub(5, 34).Results.AssertEquals(Expected);
}
