namespace Regression;

[TestClass, TestCategory("Regression")]
public class FisherTransformTests : RegressionTestBase<FisherTransformResult>
{
    public FisherTransformTests() : base("fisher.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToFisherTransform(10).IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => Quotes.ToFisherTransformList(10).IsExactly(Expected);

    [TestMethod]
    public override void Stream() => QuoteHub.ToFisherTransformHub(10).Results.IsExactly(Expected);
}
