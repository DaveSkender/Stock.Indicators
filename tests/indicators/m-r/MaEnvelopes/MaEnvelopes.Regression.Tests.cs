
namespace Regression;

[TestClass, TestCategory("Regression")]
public class MaenvelopesTests : RegressionTestBase<MaEnvelopeResult>
{
    public MaenvelopesTests() : base("ma-env.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToMaEnvelopes(20, 2.5).AssertEquals(Expected);

    [TestMethod]
    public override void Buffer() => Assert.Inconclusive("Buffer implementation not yet available");

    [TestMethod]
    public override void Stream() => Assert.Inconclusive("Stream implementation not yet available");
}
