namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for calculating the Price Relative Strength (PRS) series.
/// </summary>
public static partial class Prs
{
    #region Overloads

    /// <inheritdoc cref="ToPrs(IReadOnlyList{IReusable}, IReadOnlyList{IReusable}, int)" />
    public static IReadOnlyList<PrsResult> ToPrs(
        this IReadOnlyList<IReusable> sourceEval,
        IReadOnlyList<IReusable> sourceBase)
        => sourceEval.ToPrs(sourceBase, int.MinValue);
    #endregion

    /// <summary>
    /// Converts a list of evaluation source values and base source values to a list of PRS results.
    /// </summary>    
    /// <param name="sourceEval">The list of evaluation source values.</param>
    /// <param name="sourceBase">The list of base source values.</param>
    /// <param name="lookbackPeriods">
    /// The number of periods for the PRS% lookback calculation.  Optional.
    /// </param>
    /// <returns>A list of PRS results.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the source list is null.</exception>
    /// <exception cref="InvalidQuotesException">Thrown when the timestamp sequence does not match.</exception>
    public static IReadOnlyList<PrsResult> ToPrs(
        this IReadOnlyList<IReusable> sourceEval,
        IReadOnlyList<IReusable> sourceBase,
        int lookbackPeriods)
    {
        // check parameter arguments
        ArgumentNullException.ThrowIfNull(sourceEval);
        ArgumentNullException.ThrowIfNull(sourceBase);
        Validate(sourceEval, sourceBase, lookbackPeriods);

        // initialize
        int length = sourceEval.Count;
        List<PrsResult> results = new(length);

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            IReusable b = sourceBase[i];
            IReusable e = sourceEval[i];

            if (e.Timestamp != b.Timestamp)
            {
                throw new InvalidQuotesException(
                    nameof(sourceEval), e.Timestamp,
                    "Timestamp sequence does not match.  "
                  + "Price Relative requires matching dates in provided histories.");
            }

            double? prsPercent = null;

            if (lookbackPeriods > 0 && i > lookbackPeriods - 1)
            {
                IReusable bo = sourceBase[i - lookbackPeriods];
                IReusable eo = sourceEval[i - lookbackPeriods];

                if (bo.Value != 0 && eo.Value != 0)
                {
                    double? pctB = (b.Value - bo.Value) / bo.Value;
                    double? pctE = (e.Value - eo.Value) / eo.Value;

                    prsPercent = (pctE - pctB).NaN2Null();
                }
            }

            PrsResult r = new(
                Timestamp: e.Timestamp,

                Prs: b.Value == 0
                    ? null
                    : (e.Value / b.Value).NaN2Null(), // relative strength ratio

                PrsPercent: prsPercent);

            results.Add(r);
        }

        return results;
    }
}
