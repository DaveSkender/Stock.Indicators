namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // select element
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="TupleSelect"]/*' />
    ///
    public static IEnumerable<(DateTime Date, double? Value)> ToTupleResult(
        this IEnumerable<AdlResult> results,
        string element)
        => results
          .Select(r => element switch
          {
              "MoneyFlowMultiplier" => (r.Date, r.MoneyFlowMultiplier),
              "MoneyFlowVolume" => (r.Date, r.MoneyFlowVolume),
              "Adl" => (r.Date, r.Adl),
              "AdlSma" => (r.Date, r.AdlSma),
              _ => throw new ArgumentOutOfRangeException(nameof(element), "Unknown element provided."),
          });
}
