namespace Regression;

[TestClass, TestCategory("Regression")]
public class TrixTests : RegressionTestBase<TrixResult>
{
    public TrixTests() : base("trix.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToTrix().IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => Quotes.ToTrixList(14).IsExactly(Expected);

    [TestMethod]
    public override void Stream() => QuoteHub.ToTrixHub(14).Results.IsExactly(Expected);
}
