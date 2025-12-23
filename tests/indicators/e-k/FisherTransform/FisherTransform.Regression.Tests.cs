namespace Regression;

[TestClass, TestCategory("Regression")]
public class FisherTransformTests : RegressionTestBase<FisherTransformResult>
{
    public FisherTransformTests() : base("fisher.standard.json") { }

    [TestMethod]
    public override void Series() =>
        // FisherTransform uses recursive calculations (Fisher[i] = f(Fisher[i-1]))
        // which accumulate floating-point precision differences at ~14-16th decimal place
        Quotes.ToFisherTransform(10).IsApproximately(Expected);

    [TestMethod]
    public override void Buffer() => Assert.Inconclusive("Stream implementation not yet available");

    [TestMethod]
    public override void Stream() => Assert.Inconclusive("Stream implementation not yet available");
}
