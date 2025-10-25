namespace Regression;

[TestClass, TestCategory("Regression")]
public class PivotPointsTests : RegressionTestBase<PivotPointsResult>
{
    public PivotPointsTests() : base("pivot-points.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToPivotPoints().AssertEquals(Expected);

    [TestMethod]
    public override void Buffer() => Assert.Inconclusive("Buffer implementation not yet available");

    [TestMethod]
    public override void Stream() => Assert.Inconclusive("Stream implementation not yet available");
}
