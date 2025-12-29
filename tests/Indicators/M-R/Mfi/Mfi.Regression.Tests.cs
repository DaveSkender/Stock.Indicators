namespace Regression;

[TestClass, TestCategory("Regression")]
public class MfiTests : RegressionTestBase<MfiResult>
{
    public MfiTests() : base("mfi.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToMfi(14).IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => Quotes.ToMfiList(14).IsExactly(Expected);

    [TestMethod]
    public override void Stream() => Quotes.ToMfiHub(14).Results.IsExactly(Expected);
}
