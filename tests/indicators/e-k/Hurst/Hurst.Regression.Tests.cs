namespace Regression;

[TestClass, TestCategory("Regression")]
public class HurstTests : RegressionTestBase<HurstResult>
{
    public HurstTests() : base("hurst.standard.json") { }

    [TestMethod]
    public override void Series() =>
        // Hurst uses complex statistical calculations that accumulate
        // floating-point precision differences at ~16th decimal place
        Quotes.ToHurst().IsApproximately(Expected);

    [TestMethod]
    public override void Buffer() =>
        // Hurst uses complex statistical calculations that accumulate
        // floating-point precision differences at ~16th decimal place
        Quotes.ToHurstList().IsApproximately(Expected);

    [TestMethod]
    public override void Stream() =>
        // Hurst uses complex statistical calculations that accumulate
        // floating-point precision differences at ~16th decimal place
        Quotes.ToHurstHub().Results.IsApproximately(Expected);
}
