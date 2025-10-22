namespace Regression;

[TestClass, TestCategory("Regression")]
public class HeikinashiTests : RegressionTestBase<HeikinAshiResult>
{
    public HeikinashiTests() : base("heikinashi.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToHeikinAshi().AssertEquals(Expected);

    [TestMethod]
    public override void Buffer() => new HeikinAshiList() { Quotes }.AssertEquals(Expected);

    [TestMethod]
    public override void Stream() => Quotes.ToHeikinAshiHub().Results.AssertEquals(Expected);
}
