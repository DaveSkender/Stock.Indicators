namespace Regression;

[TestClass, TestCategory("Regression")]
public class ConnorsrsiTests : RegressionTestBase<ConnorsRsiResult>
{
    public ConnorsrsiTests() : base("crsi.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToConnorsRsi(3, 2, 100).AssertEquals(Expected);

    [TestMethod]
    public override void Buffer() => Assert.Inconclusive("Buffer implementation not yet available");

    [TestMethod]
    public override void Stream() => Assert.Inconclusive("Stream implementation not yet available");
}
