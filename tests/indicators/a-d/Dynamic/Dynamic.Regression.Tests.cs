namespace Regression;

[TestClass, TestCategory("Regression")]
public class DynamicTests : RegressionTestBase<DynamicResult>
{
    public DynamicTests() : base("dynamic.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToDynamic(14).AssertEquals(Expected);

    [TestMethod]
    public override void Buffer() => new DynamicList(14, 0.6, Quotes).AssertEquals(Expected);

    [TestMethod]
    public override void Stream() => Assert.Inconclusive("Stream implementation not yet available");
}
