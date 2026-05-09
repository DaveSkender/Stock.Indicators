namespace Regression;

[TestClass, TestCategory("Regression")]
public class UlcerindexTests : RegressionTestBase<UlcerIndexResult>
{
    public UlcerindexTests() : base("ulcer.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToUlcerIndex(14).IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => Quotes.ToUlcerIndexList(14).IsExactly(Expected);

    [TestMethod]
    public override void Stream() => QuoteHub.ToUlcerIndexHub(14).Results.IsExactly(Expected);
}
