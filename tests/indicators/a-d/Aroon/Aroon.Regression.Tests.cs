namespace Regression;

[TestClass, TestCategory("Regression")]
public class AroonTests : RegressionTestBase<AroonResult>
{
    public AroonTests() : base("aroon.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToAroon(25).IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => Quotes.ToAroonList(25).IsExactly(Expected);

    [TestMethod]
    public override void Stream() => QuoteHub.ToAroonHub(25).Results.IsExactly(Expected);
}
