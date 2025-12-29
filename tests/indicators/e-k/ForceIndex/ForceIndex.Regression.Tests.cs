namespace Regression;

[TestClass, TestCategory("Regression")]
public class ForceIndexTests : RegressionTestBase<ForceIndexResult>
{
    public ForceIndexTests() : base("force.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToForceIndex().IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => Quotes.ToForceIndexList(2).IsExactly(Expected);

    [TestMethod]
    public override void Stream() => Quotes.ToForceIndexHub().Results.IsExactly(Expected);
}
