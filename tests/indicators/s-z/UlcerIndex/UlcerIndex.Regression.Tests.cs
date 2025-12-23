namespace Regression;

[TestClass, TestCategory("Regression")]
public class UlcerindexTests : RegressionTestBase<UlcerIndexResult>
{
    public UlcerindexTests() : base("ulcer.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToUlcerIndex(14).IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => Assert.Inconclusive("Buffer implementation not yet available");

    [TestMethod]
    public override void Stream() => Assert.Inconclusive("Stream implementation not yet available");
}
