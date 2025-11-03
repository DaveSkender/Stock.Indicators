namespace Regression;

[TestClass, TestCategory("Regression")]
public class StddevchannelsTests : RegressionTestBase<StdDevChannelsResult>
{
    public StddevchannelsTests() : base("stdev-channels.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToStdDevChannels(20).AssertEquals(Expected);

    public override void Buffer() => throw new NotImplementedException("Intentionally not implemented");
    public override void Stream() => throw new NotImplementedException("Intentionally not implemented");
}
