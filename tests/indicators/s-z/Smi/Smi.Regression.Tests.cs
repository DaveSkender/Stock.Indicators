namespace Regression;

[TestClass, TestCategory("Regression")]
public class SmiTests : RegressionTestBase<SmiResult>
{
    public SmiTests() : base("smi.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToSmi().IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => Quotes.ToSmiList().IsExactly(Expected);

    [TestMethod]
    public override void Stream() => QuoteHub.ToSmiHub().Results.IsExactly(Expected);
}
