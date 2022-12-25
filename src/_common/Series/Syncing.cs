using System.Collections.ObjectModel;

namespace Skender.Stock.Indicators;

// SYNCHRONIZING SERIES
public static class Syncing
{
    // SYNC INDEX - RESIZE TO MATCH OTHER
    public static IEnumerable<TSeriesA> SyncIndex<TSeriesA, TSeriesB>(
        this IEnumerable<TSeriesA> seriesA,
        IEnumerable<TSeriesB> seriesB,
        SyncType syncType = SyncType.FullMatch)
        where TSeriesA : ISeries
        where TSeriesB : ISeries
    {
        // initialize
        List<TSeriesA> resultsList = seriesA.ToSortedList();
        List<TSeriesB> matchList = seriesB.ToSortedList();

        if (matchList.Count == 0 || resultsList.Count == 0)
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

        Type type = resultsList[0].GetType();

        // add plugs for missing values
        if (prepend || append)
        {
            List<TSeriesA> toAppend = new();

            for (int i = 0; i < matchList.Count; i++)
            {
                TSeriesB? m = matchList[i];
                TSeriesA? r = resultsList.Find(m.Date);

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

            resultsList.AddRange(toAppend);
        }

        // remove unmatched results
        if (remove)
        {
            List<TSeriesA> toRemove = new();

            for (int i = 0; i < resultsList.Count; i++)
            {
                TSeriesA? r = resultsList[i];
                TSeriesB? m = matchList.Find(r.Date);

                if (m is null)
                {
                    toRemove.Add(r);
                }
            }

            _ = resultsList.RemoveAll(x => toRemove.Contains(x));
        }

        return resultsList.ToSortedList();
    }
}
