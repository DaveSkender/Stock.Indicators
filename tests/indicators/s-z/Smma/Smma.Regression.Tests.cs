namespace Regression;

[TestClass, TestCategory("Regression")]
public class SmmaTests : RegressionTestBase<SmmaResult>
{
    public SmmaTests() : base("smma.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToSmma(20).IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => Quotes.ToSmmaList(20).IsExactly(Expected);

    [TestMethod]
    public override void Stream() => QuoteHub.ToSmmaHub(20).Results.IsExactly(Expected);
}
