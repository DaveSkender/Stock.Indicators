namespace Regression;

[TestClass, TestCategory("Regression")]
public class TemaTests : RegressionTestBase<TemaResult>
{
    public TemaTests() : base("tema.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToTema(20).AssertEquals(Expected);

    [TestMethod]
    public override void Buffer() => Assert.Inconclusive("Buffer implementation not yet available");

    [TestMethod]
    public override void Stream() => Assert.Inconclusive("Stream implementation not yet available");
}
