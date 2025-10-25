namespace Regression;

[TestClass, TestCategory("Regression")]
public class SmaAnalysisTests : RegressionTestBase<SmaAnalysisResult>
{
    public SmaAnalysisTests() : base("sma-analysis.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToSmaAnalysis(20).AssertEquals(Expected);

    [TestMethod]
    public override void Buffer() => Assert.Inconclusive("Buffer implementation not yet available");

    [TestMethod]
    public override void Stream() => Assert.Inconclusive("Stream implementation not yet available");
}
