namespace Regression;

[TestClass, TestCategory("Regression")]
public class ConnorsRsiTests : RegressionTestBase<ConnorsRsiResult>
{
    public ConnorsRsiTests() : base("crsi.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToConnorsRsi(3, 2, 100).AssertEquals(Expected);

    [TestMethod]
    public override void Buffer() => Quotes.ToConnorsRsiList(3, 2, 100).AssertEquals(Expected);

    [TestMethod]
    public override void Stream() => Assert.Inconclusive("Stream implementation not yet available");
}
