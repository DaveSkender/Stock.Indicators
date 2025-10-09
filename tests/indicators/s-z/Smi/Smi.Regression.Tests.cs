
namespace Regression;

[TestClass, TestCategory("Regression")]
public class SmiTests : RegressionTestBase<SmiResult>
{
    public SmiTests() : base("smi.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToSmi(14, 20, 5, 3).AssertEquals(Expected);

    [TestMethod]
    public override void Buffer() => Assert.Inconclusive("Buffer implementation not yet available");

    [TestMethod]
    public override void Stream() => Assert.Inconclusive("Stream implementation not yet available");
}
