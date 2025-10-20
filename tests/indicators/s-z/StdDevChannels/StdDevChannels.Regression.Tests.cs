namespace Regression;

[TestClass, TestCategory("Regression")]
public class StddevchannelsTests : RegressionTestBase<StdDevChannelsResult>
{
    public StddevchannelsTests() : base("stdev-channels.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToStdDevChannels(20).AssertEquals(Expected);

    [TestMethod]
    public override void Buffer() => Quotes.ToStdDevChannelsList(20).AssertEquals(Expected);

    [TestMethod]
    public override void Stream() => Assert.Inconclusive("Stream implementation not yet available");
}
