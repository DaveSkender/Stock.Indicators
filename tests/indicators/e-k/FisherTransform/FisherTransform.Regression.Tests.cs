namespace Regression;

[TestClass, TestCategory("Regression")]
public class FisherTransformTests : RegressionTestBase<FisherTransformResult>
{
    public FisherTransformTests() : base("fisher.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToFisherTransform(10).AssertEquals(Expected);

    [TestMethod]
    public override void Buffer() => new FisherTransformList(10, Quotes).AssertEquals(Expected);

    [TestMethod]
    public override void Stream() => Assert.Inconclusive("Stream implementation not yet available");
}
