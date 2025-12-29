namespace Regression;

[TestClass, TestCategory("Regression")]
public class FcbTests : RegressionTestBase<FcbResult>
{
    public FcbTests() : base("fcb.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToFcb(2).IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => Quotes.ToFcbList(2).IsExactly(Expected);

    [TestMethod]
    public override void Stream() => Quotes.ToFcbHub(2).Results.IsExactly(Expected);
}
