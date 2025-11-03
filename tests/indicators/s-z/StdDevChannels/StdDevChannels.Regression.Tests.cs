namespace Regression;

[TestClass, TestCategory("Regression")]
public class StddevchannelsTests : RegressionTestBase<StdDevChannelsResult>
{
    public StddevchannelsTests() : base("stdev-channels.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToStdDevChannels(20).AssertEquals(Expected);

    [TestMethod]
    public override void Buffer() => Assert.Inconclusive("Buffer implementation not available due to repaint-by-design algorithm");

    [TestMethod]
    public override void Stream() => Assert.Inconclusive("Stream implementation not available due to repaint-by-design algorithm");
}
