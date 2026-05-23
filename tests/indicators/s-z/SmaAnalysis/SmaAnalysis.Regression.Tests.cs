namespace Regression;

[TestClass, TestCategory("Regression")]
public class SmaAnalysisTests : RegressionTestBase<SmaAnalysisResult>
{
    public SmaAnalysisTests() : base("sma-analysis.standard.json") { }

    [TestMethod]
    public override void Series_AgainstBaseline_MatchesExactly() => Quotes.ToSmaAnalysis(20).IsExactly(Expected);

    [TestMethod]
    public override void Buffer_AgainstBaseline_MatchesExactly() => Quotes.ToSmaAnalysisList(20).IsExactly(Expected);

    [TestMethod]
    public override void Stream_AgainstBaseline_MatchesExactly() => Quotes.ToSmaAnalysisHub(20).Results.IsExactly(Expected);
}
