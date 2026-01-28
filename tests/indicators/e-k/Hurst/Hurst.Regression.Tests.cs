namespace Regression;

[TestClass, TestCategory("Regression")]
public class HurstTests : RegressionTestBase<HurstResult>
{
    public HurstTests() : base("hurst.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToHurst().IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => Quotes.ToHurstList().IsExactly(Expected);

    [TestMethod]
    public override void Stream() => Quotes.ToHurstHub().Results.IsExactly(Expected);
}
