namespace Regression;

[TestClass, TestCategory("Regression")]
public class ElderrayTests : RegressionTestBase<ElderRayResult>
{
    public ElderrayTests() : base("elder-ray.standard.json") { }

    [TestMethod]
    public override void Series_AgainstBaseline_MatchesExactly() => Quotes.ToElderRay(13).IsExactly(Expected);

    [TestMethod]
    public override void Buffer_AgainstBaseline_MatchesExactly() => Quotes.ToElderRayList(13).IsExactly(Expected);

    [TestMethod]
    public override void Stream_AgainstBaseline_MatchesExactly() => Quotes.ToElderRayHub(13).Results.IsExactly(Expected);
}
