namespace Regression;

[TestClass, TestCategory("Regression")]
public class MarubozuTests : RegressionTestBase<CandleResult>
{
    public MarubozuTests() : base("marubozu.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToMarubozu().IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => Assert.Inconclusive("Buffer implementation not yet available");

    [TestMethod]
    public override void Stream() => Assert.Inconclusive("Stream implementation not yet available");
}
