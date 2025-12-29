namespace Regression;

[TestClass, TestCategory("Regression")]
public class StochTests : RegressionTestBase<StochResult>
{
    public StochTests() : base("stoch.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToStoch(14, 3, 3).IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => Quotes.ToStochList(14, 3, 3).IsExactly(Expected);

    [TestMethod]
    public override void Stream() => QuoteHub.ToStochHub(14, 3, 3).Results.IsExactly(Expected);
}
