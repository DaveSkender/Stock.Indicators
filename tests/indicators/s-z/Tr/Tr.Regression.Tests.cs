namespace Regression;

[TestClass, TestCategory("Regression")]
public class TrTests : RegressionTestBase<TrResult>
{
    public TrTests() : base("tr.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToTr().IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => Assert.Inconclusive("Buffer implementation not yet available");

    [TestMethod]
    public override void Stream() => Assert.Inconclusive("Stream implementation not yet available");
}
