namespace Regression;

[TestClass, TestCategory("Regression")]
public class FisherTransformTests : RegressionTestBase<FisherTransformResult>
{
    public FisherTransformTests() : base("fisher.standard.json") { }

    [TestMethod]
    public override void Series_AgainstBaseline_MatchesExactly() => Quotes.ToFisherTransform(10).IsExactly(Expected);

    [TestMethod]
    public override void Buffer_AgainstBaseline_MatchesExactly() => Quotes.ToFisherTransformList(10).IsExactly(Expected);

    [TestMethod]
    public override void Stream_AgainstBaseline_MatchesExactly() => QuoteHub.ToFisherTransformHub(10).Results.IsExactly(Expected);
}
