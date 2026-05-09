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
}
