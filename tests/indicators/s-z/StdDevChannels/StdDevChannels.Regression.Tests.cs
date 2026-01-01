namespace Regression;

[TestClass, TestCategory("Regression")]
public class StddevchannelsTests : RegressionTestBase<StdDevChannelsResult>
{
    public StddevchannelsTests() : base("stdev-channels.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToStdDevChannels(20).IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => Assert.Inconclusive("Test not yet implemented");

    [TestMethod]
    public override void Stream() => Assert.Inconclusive("Test not yet implemented");
}
