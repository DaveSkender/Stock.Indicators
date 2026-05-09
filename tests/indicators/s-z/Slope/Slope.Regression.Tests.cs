namespace Regression;

[TestClass, TestCategory("Regression")]
public class SlopeTests : RegressionTestBase<SlopeResult>
{
    public SlopeTests() : base("slope.standard.json") { }

    private const int n = 14;

    [TestMethod]
    public override void Series() => Quotes.ToSlope(n).IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => Quotes.ToSlopeList(n).IsExactly(Expected);

    [TestMethod]
    public override void Stream() => Quotes.ToSlopeHub(n).Results.IsExactly(Expected);
}
