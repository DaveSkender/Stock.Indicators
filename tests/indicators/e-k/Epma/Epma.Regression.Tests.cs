namespace Regression;

[TestClass, TestCategory("Regression")]
public class EpmaTests : RegressionTestBase<EpmaResult>
{
    public EpmaTests() : base("epma.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToEpma().IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => Quotes.ToEpmaList(10).IsExactly(Expected);

    [TestMethod]
    public override void Stream() => QuoteHub.ToEpmaHub(10).Results.IsExactly(Expected);
}
