namespace Regression;

[TestClass, TestCategory("Regression")]
public class ForceIndexTests : RegressionTestBase<ForceIndexResult>
{
    public ForceIndexTests() : base("force.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToForceIndex().AssertEquals(Expected);

    [TestMethod]
    public override void Buffer() => new ForceIndexList(2, Quotes).AssertEquals(Expected);

    [TestMethod]
    public override void Stream() => Assert.Inconclusive("Stream implementation not yet available");
}
