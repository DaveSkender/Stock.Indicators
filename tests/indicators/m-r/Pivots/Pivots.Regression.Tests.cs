namespace Regression;

[TestClass, TestCategory("Regression")]
public class PivotsTests : RegressionTestBase<PivotsResult>
{
    public PivotsTests() : base("pivots.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToPivots().AssertEquals(Expected);

    [TestMethod]
    public override void Buffer() => Quotes.ToPivotsList().AssertEquals(Expected);

    [TestMethod]
    public override void Stream() => Assert.Inconclusive("Stream implementation not yet available");
}
