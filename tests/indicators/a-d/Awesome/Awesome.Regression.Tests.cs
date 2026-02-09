namespace Regression;

[TestClass, TestCategory("Regression")]
public class AwesomeTests : RegressionTestBase<AwesomeResult>
{
    public AwesomeTests() : base("awesome.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToAwesome(5, 34).IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => Quotes.ToAwesomeList(5, 34).IsExactly(Expected);

    [TestMethod]
    public override void Stream() => QuoteHub.ToAwesomeHub(5, 34).Results.IsExactly(Expected);
}
