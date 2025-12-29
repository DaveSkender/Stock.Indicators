using System.Globalization;

namespace Skender.Stock.Indicators;

/// <summary>
/// Provides utility methods for the PRS (Price Relative Strength) indicator.
/// </summary>
public static partial class Prs
{
    private static readonly CultureInfo invariantCulture = CultureInfo.InvariantCulture;

    /// <summary>
    /// Validates the parameters for PRS calculations.
    /// </summary>
    /// <typeparam name="T">The type of the source values.</typeparam>
    /// <param name="quotesEval">The list of evaluation quotes.</param>
    /// <param name="quotesBase">The list of base quotes.</param>
    /// <param name="lookbackPeriods">The number of periods for the lookback calculation.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are less than or equal to 0.</exception>
    /// <exception cref="InvalidQuotesException">Thrown when there are insufficient quotes or mismatched quote counts.</exception>
    internal static void Validate<T>(
        IReadOnlyList<T> quotesEval,
        IReadOnlyList<T> quotesBase,
        int? lookbackPeriods)
        where T : IReusable
    {
        // check parameter arguments
        if (lookbackPeriods is <= 0 and not int.MinValue)
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
