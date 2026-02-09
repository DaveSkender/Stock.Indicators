namespace Regression;

[TestClass, TestCategory("Regression")]
public class StdDevChannelsTests : RegressionTestBase<StdDevChannelsResult>
{
    public StdDevChannelsTests() : base("stdev-channels.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToStdDevChannels(20).IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => Assert.Inconclusive("Test not yet implemented");
    // TODO: BufferList implementation not available for StdDevChannels

    [TestMethod]
    public override void Stream() => Assert.Inconclusive("Test not yet implemented");
    // TODO: StreamHub implementation not available for StdDevChannels
}
