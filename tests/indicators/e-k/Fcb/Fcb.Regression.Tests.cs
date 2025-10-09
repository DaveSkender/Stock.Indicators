namespace Regression;

[TestClass, TestCategory("Regression")]
public class FcbTests : RegressionTestBase<FcbResult>
{
    public FcbTests() : base("fcb.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToFcb(2).AssertEquals(Expected);

    [TestMethod]
    public override void Buffer() => new FcbList(2, Quotes).AssertEquals(Expected);

    [TestMethod]
    public override void Stream() => Assert.Inconclusive("Stream implementation not yet available");
}
