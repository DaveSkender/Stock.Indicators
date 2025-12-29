namespace Regression;

[TestClass, TestCategory("Regression")]
public class RollingPivotsTests : RegressionTestBase<RollingPivotsResult>
{
    public RollingPivotsTests() : base("rolling-pivots.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToRollingPivots().IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => Quotes.ToRollingPivotsList().IsExactly(Expected);

    [TestMethod]
    public override void Stream() => QuoteHub.ToRollingPivotsHub().Results.IsExactly(Expected);
}
