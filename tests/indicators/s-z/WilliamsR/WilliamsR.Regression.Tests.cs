namespace Regression;

[TestClass, TestCategory("Regression")]
public class WilliamsrTests : RegressionTestBase<WilliamsResult>
{
    public WilliamsrTests() : base("willr.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToWilliamsR(14).IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => Quotes.ToWilliamsRList(14).IsExactly(Expected);

    [TestMethod]
    public override void Stream() => QuoteHub.ToWilliamsRHub(14).Results.IsExactly(Expected);
}
