namespace Regression;

[TestClass, TestCategory("Regression")]
public class DonchianTests : RegressionTestBase<DonchianResult>
{
    public DonchianTests() : base("donchian.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToDonchian(20).IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => Quotes.ToDonchianList(20).IsExactly(Expected);

    [TestMethod]
    public override void Stream() => QuoteHub.ToDonchianHub(20).Results.IsExactly(Expected);
}
