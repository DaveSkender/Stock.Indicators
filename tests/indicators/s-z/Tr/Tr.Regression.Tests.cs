namespace Regression;

[TestClass, TestCategory("Regression")]
public class TrTests : RegressionTestBase<TrResult>
{
    public TrTests() : base("tr.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToTr().IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => Quotes.ToTrList().IsExactly(Expected);

    [TestMethod]
    public override void Stream() => QuoteHub.ToTrHub().Results.IsExactly(Expected);
}
