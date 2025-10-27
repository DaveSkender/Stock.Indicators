namespace Regression;

[TestClass, TestCategory("Regression")]
public class GatorTests : RegressionTestBase<GatorResult>
{
    public GatorTests() : base("gator.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToGator().AssertEquals(Expected);

    [TestMethod]
    public override void Buffer() => new GatorList { Quotes }.AssertEquals(Expected);

    [TestMethod]
    public override void Stream() => Quotes.ToGatorHub().Results.AssertEquals(Expected);
}
