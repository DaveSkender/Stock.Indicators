namespace Regression;

[TestClass, TestCategory("Regression")]
public class GatorTests : RegressionTestBase<GatorResult>
{
    public GatorTests() : base("gator.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToGator().IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => Quotes.ToGatorList().IsExactly(Expected);

    [TestMethod]
    public override void Stream() => Quotes.ToGatorHub().Results.IsExactly(Expected);
}
