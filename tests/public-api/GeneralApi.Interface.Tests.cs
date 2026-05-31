using Test.Tools;

namespace GeneralApi;

// PUBLIC API (INTERFACES)

[TestClass, TestCategory("Integration")]
public class UserInterface
{
    private static readonly IReadOnlyList<Quote> quotes = Data.GetDefault();
    private static readonly IReadOnlyList<Quote> quotesBad = Data.GetBad();

    [TestMethod]
    public void QuoteValidation()
    {
        IReadOnlyList<Quote> clean = quotes;

        clean.Validate();
        clean.ToSma(6);
        clean.ToEma(5);

        IReadOnlyList<Quote> reverse = quotes
            .OrderByDescending(x => x.Timestamp)
            .ToList();

        // has duplicates
        InvalidQuotesException dx
            = Assert.ThrowsExactly<InvalidQuotesException>(
                () => quotesBad.Validate());

        dx.Message.Should().Contain("Duplicate date found");

        // out of order
        InvalidQuotesException sx
            = Assert.ThrowsExactly<InvalidQuotesException>(
                () => reverse.Validate());

        sx.Message.Should().Contain("Quotes are out of sequence");
    }

    [TestMethod]
    public void QuoteValidationReturn()
    {
        IReadOnlyList<Quote> h = quotes.Validate();

        Quote f = h[0];
        Console.WriteLine($"Quote:{f}");
    }
}
