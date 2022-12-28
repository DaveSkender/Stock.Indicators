namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // select element
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Select"]/*' />
    ///
    public static IEnumerable<(DateTime Date, double? Value)> ToTupleResult(
        this IEnumerable<AdxResult> results,
        string element) => results
          .Select(r => element switch
          {
            "MoneyFlowMultiplier" => (r.Date, r.MoneyFlowMultiplier),
            "MoneyFlowVolume" => (r.Date, r.MoneyFlowVolume),
            "Adl" => (r.Date, r.Adl),
            "AdlSma" => (r.Date, r.AdlSma)
          });
}
