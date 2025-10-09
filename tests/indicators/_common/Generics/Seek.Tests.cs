namespace Utilities;

[TestClass]
public class Seeking : TestBase
{
    [TestMethod]
    public void Find()
    {
        IReadOnlyList<EmaResult> emaResults = Quotes.ToEma(20);

        // find specific date
        DateTime findDate = DateTime.ParseExact("2018-12-31", "yyyy-MM-dd", invariantCulture);

        EmaResult r = emaResults.Find(findDate);
        r.Ema.Round(4).Should().Be(249.3519);
    }
}
