using System.Globalization;

namespace Skender.Stock.Indicators;

// PRICE RELATIVE STRENGTH (UTILITIES)

public static partial class Prs
{
    private static readonly CultureInfo invariantCulture = CultureInfo.InvariantCulture;

    // parameter validation
    internal static void Validate<T>(
        IReadOnlyList<T> quotesEval,
        IReadOnlyList<T> quotesBase,
        int? lookbackPeriods)
        where T : IReusable
    {
        // check parameter arguments
        if (lookbackPeriods is <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for Price Relative Strength.");
        }

        // check quotes
        int qtyHistoryEval = quotesEval.Count;
        int qtyHistoryBase = quotesBase.Count;

        if (qtyHistoryEval < lookbackPeriods)
        {
            string message = "Insufficient quotes provided for Price Relative Strength.  " +
                string.Format(
                    invariantCulture,
                    "You provided {0} periods of quotes when at least {1} are required.",
                    qtyHistoryEval, lookbackPeriods);

            throw new InvalidQuotesException(nameof(quotesEval), message);
        }

        if (qtyHistoryBase != qtyHistoryEval)
        {
            throw new InvalidQuotesException(
                nameof(quotesBase),
                "Base quotes should have at least as many records as Eval quotes for PRS.");
        }
    }

}