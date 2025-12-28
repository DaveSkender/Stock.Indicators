namespace Regression;

[TestClass, TestCategory("Regression")]
public class WmaTests : RegressionTestBase<WmaResult>
{
    public WmaTests() : base("wma.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToWma(14).IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => Quotes.ToWmaList(14).IsExactly(Expected);

    [TestMethod]
    public override void Stream() => QuoteHub.ToWmaHub(14).Results.IsExactly(Expected);
}
