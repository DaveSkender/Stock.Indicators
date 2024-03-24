namespace Skender.Stock.Indicators;

// BASE QUOTE (COMMON)

public static class BasicQuote
{
    // parameter validation
    internal static void Validate(Quote quote)
    {
        if (quote is null)
        {
            throw new ArgumentNullException(nameof(quote), "Quote cannot be null for BasicQuotes");
        }
    }

}
