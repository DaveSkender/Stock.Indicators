namespace Regression;

[TestClass, TestCategory("Regression")]
public class DynamicTests : RegressionTestBase<DynamicResult>
{
    public DynamicTests() : base("dynamic.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToDynamic(14).IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => Quotes.ToDynamicList(14).IsExactly(Expected);

    [TestMethod]
    public override void Stream() => Quotes.ToDynamicHub(14, 0.6).Results.IsExactly(Expected);
}
