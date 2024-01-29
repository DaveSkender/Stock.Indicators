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
        if (startDate < quotesList[0].TickDate)
        {
            throw new ArgumentOutOfRangeException(nameof(startDate), startDate,
                "Start TickDate must be within the quotes range for VWAP.");
        }
    }

}
