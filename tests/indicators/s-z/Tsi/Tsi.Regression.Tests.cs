namespace Regression;

[TestClass, TestCategory("Regression")]
public class TsiTests : RegressionTestBase<TsiResult>
{
    public TsiTests() : base("tsi.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToTsi(25, 13, 7).IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => Quotes.ToTsiList(25, 13, 7).IsExactly(Expected);

    [TestMethod]
    public override void Stream() => QuoteHub.ToTsiHub(25, 13, 7).Results.IsExactly(Expected);
}
