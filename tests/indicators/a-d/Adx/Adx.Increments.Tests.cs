namespace Increments;

[TestClass]
public class Adx : BufferListTestBase
{
    private const int lookbackPeriods = 14;

    private static readonly IReadOnlyList<AdxResult> series
       = Quotes.ToAdx(lookbackPeriods);

    [TestMethod]
    public override void FromQuote()
    {
        AdxList sut = new(lookbackPeriods);

        foreach (Quote q in Quotes) { sut.Add(q); }

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public override void FromQuoteBatch()
    {
        AdxList sut = new(lookbackPeriods) { Quotes };

        IReadOnlyList<AdxResult> series
            = Quotes.ToAdx(lookbackPeriods);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }
}
