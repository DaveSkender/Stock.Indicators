namespace Skender.Stock.Indicators;

// RESULTS UTILITIES

public static partial class ResultUtility
{
    // TODO: this may be obsolete, was renamed and kept, but possibly superfluous
    // in any case, this isn't uniquly a ResultUtility anymore

    // SYNCHRONIZING RESULTS - RESIZE TO MATCH OTHER
    /// <include file='./info.xml' path='info/type[@name="SyncResult"]/*' />
    ///
    public static IEnumerable<TSeriesA> SyncSeries<TSeriesA, TSeriesB>(
        this IEnumerable<TSeriesA> syncMe,
        IEnumerable<TSeriesB> toMatch,
        SyncType syncType = SyncType.FullMatch)
        where TSeriesA : ISeries
        where TSeriesB : ISeries
    {
        // initialize
        List<TSeriesA> syncMeList = syncMe.ToSortedList();
        List<TSeriesB> toMatchList = toMatch.ToSortedList();

        if (syncMeList.Count == 0 || toMatchList.Count == 0)
        {
            return new List<TSeriesA>();
        }

        bool prepend = false;
        bool append = false;
        bool remove = false;

        switch (syncType)
        {
            case SyncType.Prepend:
                prepend = true;
                break;

            case SyncType.AppendOnly:
                prepend = append = true;
                break;

            case SyncType.RemoveOnly:
                remove = true;
                break;

            case SyncType.FullMatch:
                prepend = append = remove = true;
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(syncType));
        }

        Type type = syncMeList[0].GetType();

        // add plugs for missing values
        if (prepend || append)
        {
            List<TSeriesA> toAppend = [];

            for (int i = 0; i < toMatchList.Count; i++)
            {
                TSeriesB? m = toMatchList[i];
                TSeriesA? r = syncMeList.Find(m.Date);

                if (r is null)
                {
                    TSeriesA? n = (TSeriesA?)Activator.CreateInstance(type, m.Date);
                    if (n != null)
                    {
                        toAppend.Add(n);
                    }
                }
                else if (!append)
                {
                    break;
                }
            }

            syncMeList.AddRange(toAppend);
        }

        // remove unmatched results
        if (remove)
        {
            List<TSeriesA> toRemove = [];

            for (int i = 0; i < syncMeList.Count; i++)
            {
                TSeriesA? r = syncMeList[i];
                TSeriesB? m = toMatchList.Find(r.Date);

                if (m is null)
                {
                    toRemove.Add(r);
                }
            }

            syncMeList.RemoveAll(x => toRemove.Contains(x));
        }

        return syncMeList.ToSortedList();
    }
}
