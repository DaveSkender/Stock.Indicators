namespace Regression;

[TestClass, TestCategory("Regression")]
public class CmoTests : RegressionTestBase<CmoResult>
{
    public CmoTests() : base("cmo.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToCmo(14).IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => Quotes.ToCmoList(14).IsExactly(Expected);

    [TestMethod]
    public override void Stream() => QuoteHub.ToCmoHub(14).Results.IsExactly(Expected);
}
