
namespace Regression;

[TestClass, TestCategory("Regression")]
public class IchimokuTests : RegressionTestBase<IchimokuResult>
{
    public IchimokuTests() : base("ichimoku.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToIchimoku(9, 26, 52).AssertEquals(Expected);

    [TestMethod]
    public override void Buffer() => Assert.Inconclusive("Buffer implementation not yet available");

    [TestMethod]
    public override void Stream() => Assert.Inconclusive("Stream implementation not yet available");
}
