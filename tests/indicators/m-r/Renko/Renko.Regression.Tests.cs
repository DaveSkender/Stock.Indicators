namespace Regression;

[TestClass, TestCategory("Regression")]
public class RenkoTests : RegressionTestBase<RenkoResult>
{
    public RenkoTests() : base("renko.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToRenko(1.0m).IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => Quotes.ToRenkoList(1.0m).IsExactly(Expected);

    [TestMethod]
    public override void Stream() => QuoteHub.ToRenkoHub(1.0m).Results.IsExactly(Expected);
}
