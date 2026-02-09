namespace Regression;

[TestClass, TestCategory("Regression")]
public class TemaTests : RegressionTestBase<TemaResult>
{
    public TemaTests() : base("tema.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToTema(20).IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => Quotes.ToTemaList(20).IsExactly(Expected);

    [TestMethod]
    public override void Stream() => QuoteHub.ToTemaHub(20).Results.IsExactly(Expected);
}
