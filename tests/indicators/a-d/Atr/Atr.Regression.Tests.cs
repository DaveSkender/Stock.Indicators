namespace Regression;

[TestClass, TestCategory("Regression")]
public class AtrTests : RegressionTestBase<AtrResult>
{
    public AtrTests() : base("atr.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToAtr(14).IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => Assert.Inconclusive("Buffer implementation not yet available");

    [TestMethod]
    public override void Stream() => Assert.Inconclusive("Stream implementation not yet available");
}
