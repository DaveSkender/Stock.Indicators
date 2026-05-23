namespace Regression;

[TestClass, TestCategory("Regression")]
public class DpoTests : RegressionTestBase<DpoResult>
{
    public DpoTests() : base("dpo.standard.json") { }

    private const int lookbackPeriods = 14;

    [TestMethod]
    public override void Series_AgainstBaseline_MatchesExactly()
        => Quotes.ToDpo(lookbackPeriods).IsExactly(Expected);

    [TestMethod]
    public override void Buffer_AgainstBaseline_MatchesExactly()
        => Quotes.ToDpoList(lookbackPeriods).IsExactly(Expected);


    [TestMethod]
    public override void Stream_AgainstBaseline_MatchesExactly()
        => Quotes.ToDpoHub(lookbackPeriods).Results.IsExactly(Expected);
}
