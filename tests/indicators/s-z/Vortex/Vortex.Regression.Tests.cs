namespace Regression;

[TestClass, TestCategory("Regression")]
public class VortexTests : RegressionTestBase<VortexResult>
{
    public VortexTests() : base("vortex.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToVortex(14).IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => new VortexList(14) { Quotes }.IsExactly(Expected);

    [TestMethod]
    public override void Stream() => QuoteHub.ToVortexHub(14).Results.IsExactly(Expected);
}
