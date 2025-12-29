namespace Regression;

[TestClass, TestCategory("Regression")]
public class MaenvelopesTests : RegressionTestBase<MaEnvelopeResult>
{
    public MaenvelopesTests() : base("ma-env.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToMaEnvelopes(20, 2.5).IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => Quotes.ToMaEnvelopesList(20, 2.5).IsExactly(Expected);

    [TestMethod]
    public override void Stream() => QuoteHub.ToMaEnvelopesHub(20, 2.5).Results.IsExactly(Expected);
}
