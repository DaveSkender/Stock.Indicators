using System.Collections.ObjectModel;

namespace Skender.Stock.Indicators;

// SYNCHRONIZING SERIES
public static class Syncing
{
    // SYNC INDEX - RESIZE TO MATCH OTHER
    public static IEnumerable<TSeriesA> SyncIndex<TSeriesA, TSeriesB>(
        this IEnumerable<TSeriesA> syncMe,
        IEnumerable<TSeriesB> toMatch,
        SyncType syncType = SyncType.FullMatch)
        where TSeriesA : ISeries
        where TSeriesB : ISeries
    {
        // initialize
        List<TSeriesA> syncMeList = syncMe.ToSortedList();
        List<TSeriesB> toMatchList = toMatch.ToSortedList();

        if (toMatchList.Count == 0 || syncMeList.Count == 0)
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
        }

        Type type = syncMeList[0].GetType();

        // add plugs for missing values
        if (prepend || append)
        {
            List<TSeriesA> toAppend = new();

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
            List<TSeriesA> toRemove = new();

            for (int i = 0; i < syncMeList.Count; i++)
            {
                TSeriesA? r = syncMeList[i];
                TSeriesB? m = toMatchList.Find(r.Date);

                if (m is null)
                {
                    toRemove.Add(r);
                }
            }

            _ = syncMeList.RemoveAll(x => toRemove.Contains(x));
        }

        return syncMeList.ToSortedList();
    }
}
