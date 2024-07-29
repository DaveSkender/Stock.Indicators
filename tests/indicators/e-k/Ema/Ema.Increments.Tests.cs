namespace Increments;

[TestClass]
public class Ema : IncrementsTestBase
{
    [TestMethod]
    public override void Standard()
    {
        EmaList<Quote> sut = new(14);

        foreach (Quote quote in Quotes)
        {
            sut.Add(quote);
        }

        IReadOnlyList<EmaResult> series
            = Quotes.ToEma(14);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public override void ValueBased()
    {
        EmaArray sut = new(14);

        foreach (Quote quote in Quotes)
        {
            sut.Add(quote.Value);
        }

        List<double?> series = Quotes
            .ToEma(14)
            .Select(x => x.Ema)
            .ToList();

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }
}
