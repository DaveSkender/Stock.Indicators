namespace Regression;

[TestClass, TestCategory("Regression")]
public class DojiTests : RegressionTestBase<CandleResult>
{
    public DojiTests() : base("doji.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToDoji(0.1).IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => new DojiList(0.1) { Quotes }.IsExactly(Expected);

    [TestMethod]
    public override void Stream() => QuoteHub.ToDojiHub(0.1).Results.IsExactly(Expected);
}
