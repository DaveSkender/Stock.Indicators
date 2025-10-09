namespace Regression;

[TestClass, TestCategory("Regression")]
public class TsiTests : RegressionTestBase<TsiResult>
{
    public TsiTests() : base("tsi.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToTsi(25, 13, 7).AssertEquals(Expected);

    [TestMethod]
    public override void Buffer() => Assert.Inconclusive("Buffer implementation not yet available");

    [TestMethod]
    public override void Stream() => Assert.Inconclusive("Stream implementation not yet available");
}
