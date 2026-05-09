namespace Regression;

[TestClass, TestCategory("Regression")]
public class T3Tests : RegressionTestBase<T3Result>
{
    public T3Tests() : base("t3.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToT3(5, 0.7).IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => Quotes.ToT3List(5, 0.7).IsExactly(Expected);

    [TestMethod]
    public override void Stream() => QuoteHub.ToT3Hub(5, 0.7).Results.IsExactly(Expected);
}
