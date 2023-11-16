namespace Skender.Stock.Indicators;

// RESULTS UTILITIES

public static partial class ResultUtility
{
    // SYNCHRONIZING RESULTS - RESIZE TO MATCH OTHER
    /// <include file='./info.xml' path='info/type[@name="SyncResult"]/*' />
    ///
    public static IEnumerable<TResultA> SyncIndex<TResultA, TResultB>(
        this IEnumerable<TResultA> syncMe,
        IEnumerable<TResultB> toMatch,
        SyncType syncType = SyncType.FullMatch)
        where TResultA : ISeries
        where TResultB : ISeries
    {
        // initialize
        List<TResultA> syncMeList = syncMe.ToSortedList();
        List<TResultB> toMatchList = toMatch.ToSortedList();

        if (syncMeList.Count == 0 || toMatchList.Count == 0)
        {
            return new List<TResultA>();
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
            List<TResultA> toAppend = [];

            for (int i = 0; i < toMatchList.Count; i++)
            {
                TResultB? m = toMatchList[i];
                TResultA? r = syncMeList.Find(m.Date);

                if (r is null)
                {
                    TResultA? n = (TResultA?)Activator.CreateInstance(type, m.Date);
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
            List<TResultA> toRemove = [];

            for (int i = 0; i < syncMeList.Count; i++)
            {
                TResultA? r = syncMeList[i];
                TResultB? m = toMatchList.Find(r.Date);

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
