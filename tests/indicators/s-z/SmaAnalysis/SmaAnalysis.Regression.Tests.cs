namespace Regression;

[TestClass, TestCategory("Regression")]
public class SmaAnalysisTests : RegressionTestBase<SmaAnalysisResult>
{
    public SmaAnalysisTests() : base("sma-analysis.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToSmaAnalysis(20).AssertEquals(Expected);

    [TestMethod]
    public override void Buffer() => new SmaAnalysisList(20, Quotes).AssertEquals(Expected);

    [TestMethod]
    public override void Stream() => Quotes.ToSmaAnalysisHub(20).Results.AssertEquals(Expected);
}
