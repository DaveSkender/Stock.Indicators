namespace Regression;

[TestClass, TestCategory("Regression")]
public class CmoTests : RegressionTestBase<CmoResult>
{
    public CmoTests() : base("cmo.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToCmo(14).AssertEquals(Expected);

    [TestMethod]
    public override void Buffer() => Assert.Inconclusive("Buffer implementation not yet available");

    [TestMethod]
    public override void Stream() => Assert.Inconclusive("Stream implementation not yet available");
}
