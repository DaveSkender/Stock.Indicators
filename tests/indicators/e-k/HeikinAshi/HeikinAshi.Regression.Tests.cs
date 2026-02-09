namespace Regression;

[TestClass, TestCategory("Regression")]
public class HeikinashiTests : RegressionTestBase<HeikinAshiResult>
{
    public HeikinashiTests() : base("heikinashi.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToHeikinAshi().IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => Quotes.ToHeikinAshiList().IsExactly(Expected);

    [TestMethod]
    public override void Stream() => Quotes.ToHeikinAshiHub().Results.IsExactly(Expected);
}
