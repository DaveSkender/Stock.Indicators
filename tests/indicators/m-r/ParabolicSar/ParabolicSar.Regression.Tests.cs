namespace Regression;

[TestClass, TestCategory("Regression")]
public class ParabolicsarTests : RegressionTestBase<ParabolicSarResult>
{
    public ParabolicsarTests() : base("psar.standard.json") { }

    [TestMethod]
    public override void Series_AgainstBaseline_MatchesExactly() => Quotes.ToParabolicSar(0.02, 0.2).IsExactly(Expected);

    [TestMethod]
    public override void Buffer_AgainstBaseline_MatchesExactly() => Quotes.ToParabolicSarList(0.02, 0.2).IsExactly(Expected);

    [TestMethod]
    public override void Stream_AgainstBaseline_MatchesExactly() => Quotes.ToParabolicSarHub(0.02, 0.2).Results.IsExactly(Expected);
}
