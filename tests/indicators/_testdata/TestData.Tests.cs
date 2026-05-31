namespace TestOfTests;

[TestClass]
public class TestData : TestBase
{
    // Test the test data to
    // ensure it meets the expected format

    [TestMethod]
    public void QuotesIsValid()
    {
        Quotes.Should().HaveCount(502);
        Quotes.Validate();
    }

    [TestMethod]
    public void OtherQuotesIsValid()
    {
        OtherQuotes.Should().HaveCount(502);
        OtherQuotes.Validate();
    }

    [TestMethod]
    public void BadQuotesIsInvalid()
    {
        BadQuotes.Should().HaveCount(502);

        // duplicates
        Assert.ThrowsExactly<InvalidQuotesException>(
            static () => BadQuotes.Validate());
    }

    [TestMethod]
    public void BigQuotesIsValid()
    {
        BigQuotes.Should().HaveCount(1246);
        BigQuotes.Validate();
    }

    [TestMethod]
    public void LongishQuotesIsValid()
    {
        LongishQuotes.Should().HaveCount(5285);
        LongishQuotes.Validate();
    }

    [TestMethod]
    public void LongestQuotesIsValid()
    {
        LongestQuotes.Should().HaveCount(15821);
        LongestQuotes.Validate();
    }

    [TestMethod]
    public void MismatchQuotesIsValid()
    {
        MismatchQuotes.Should().HaveCount(502);

        // out of sequence
        Assert.ThrowsExactly<InvalidQuotesException>(
            static () => MismatchQuotes.Validate());
    }

    [TestMethod]
    public void RandomQuotesIsValid()
    {
        RandomQuotes.Should().HaveCount(1000);
        RandomQuotes.Validate();
    }

    [TestMethod]
    public void ZeroesQuotesIsValid()
    {
        ZeroesQuotes.Should().HaveCount(200);
        ZeroesQuotes.Validate();
    }

    [TestMethod]
    public void RandomGbmAllowsZeroBarsThenAddProducesData()
    {
        // bars: 0 starts an empty generator to be filled incrementally
        // via Add(DateTime) (used by the offline SSE emulator).
        Test.Data.RandomGbm generator = new(bars: 0);
        generator.Should().BeEmpty();

        DateTime timestamp = new(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        generator.Add(timestamp);

        generator.Should().ContainSingle();
        generator[0].Timestamp.Should().Be(timestamp);

        // a negative count is still rejected
        Assert.ThrowsExactly<ArgumentException>(
            static () => new Test.Data.RandomGbm(bars: -1));
    }
}
