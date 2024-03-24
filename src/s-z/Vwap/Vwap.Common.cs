namespace Skender.Stock.Indicators;

// VOLUME WEIGHTED AVERAGE PRICE (COMMON)

public static class Vwap
{
    // parameter validation
    internal static void Validate(
        List<QuoteD> quotesList,
        DateTime? startDate)
    {
        // nothing to do for 0 length
        if (quotesList.Count == 0)
        {
            return;
        }

        // check parameter arguments (intentionally after quotes check)
        if (startDate < quotesList[0].Timestamp)
        {
            throw new ArgumentOutOfRangeException(nameof(startDate), startDate,
                "Start Timestamp must be within the quotes range for VWAP.");
        }
    }

}
