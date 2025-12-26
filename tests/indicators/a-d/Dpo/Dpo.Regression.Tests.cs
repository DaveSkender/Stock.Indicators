namespace Regression;

[TestClass, TestCategory("Regression")]
public class DpoTests : RegressionTestBase<DpoResult>
{
    public DpoTests() : base("dpo.standard.json") { }

    private const int lookbackPeriods = 14;

    [TestMethod]
    public override void Series()
        => Quotes.ToDpo(lookbackPeriods).IsExactly(Expected);

    [TestMethod]
    public override void Buffer()
        => Quotes.ToDpoList(lookbackPeriods).IsExactly(Expected);


    [TestMethod]
    public override void Stream()
        => Quotes.ToDpoHub(lookbackPeriods).Results.IsExactly(Expected);
}
