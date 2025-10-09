
namespace Regression;

[TestClass, TestCategory("Regression")]
public class MfiTests : RegressionTestBase<MfiResult>
{
    public MfiTests() : base("mfi.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToMfi(14).AssertEquals(Expected);

    [TestMethod]
    public override void Buffer() => Assert.Inconclusive("Buffer implementation not yet available");

    [TestMethod]
    public override void Stream() => Assert.Inconclusive("Stream implementation not yet available");
}
