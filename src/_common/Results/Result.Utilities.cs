namespace Skender.Stock.Indicators;

// HELPER FUNCTIONS

public static partial class Indicator
{
    // FIND by DATE
    /// <include file='./info.xml' path='info/type[@name="Find"]/*' />
    ///
    public static TResult? Find<TResult>(
        this IEnumerable<TResult> results,
        DateTime lookupDate)
        where TResult : IResult => results.FirstOrDefault(x => x.Date == lookupDate);

    // REMOVE SPECIFIC PERIODS extension
    /// <include file='./info.xml' path='info/type[@name="PruneSpecific"]/*' />
    ///
    public static IEnumerable<TResult> RemoveWarmupPeriods<TResult>(
        this IEnumerable<TResult> results,
        int removePeriods)
        where TResult : IResult
        => removePeriods < 0
            ? throw new ArgumentOutOfRangeException(nameof(removePeriods), removePeriods,
                "If specified, the Remove Periods value must be greater than or equal to 0.")
            : results.Remove(removePeriods);

    // REMOVE RESULTS
    private static List<TResult> Remove<TResult>(
        this IEnumerable<TResult> results,
        int removePeriods)
        where TResult : IResult
    {
        List<TResult> resultsList = results.ToList();

        if (resultsList.Count <= removePeriods)
        {
            return new List<TResult>();
        }
        else
        {
            if (removePeriods > 0)
            {
                for (int i = 0; i < removePeriods; i++)
                {
                    resultsList.RemoveAt(0);
                }
            }

            return resultsList;
        }
    }
}
