namespace Regression;

[TestClass, TestCategory("Regression")]
public class EpmaTests : RegressionTestBase<EpmaResult>
{
    public EpmaTests() : base("epma.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToEpma().IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => new EpmaList(10) { Quotes }.IsExactly(Expected);

    [TestMethod]
    public override void Stream() => QuoteHub.ToEpmaHub(10).Results.IsExactly(Expected);
}
