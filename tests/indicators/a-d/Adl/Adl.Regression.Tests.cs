namespace Regression;

[TestClass, TestCategory("Regression")]
public class AdlTests : RegressionTestBase<AdlResult>
{
    public AdlTests() : base("adl.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToAdl().IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => Quotes.ToAdlList().IsExactly(Expected);

    [TestMethod]
    public override void Stream() => QuoteHub.ToAdlHub().Results.IsExactly(Expected);
}
