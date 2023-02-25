namespace Skender.Stock.Indicators;

// NEW HIGHS NEW LOWS (SERIES)
public static partial class Indicator
{
    internal static List<NewHighsNewLowsResult> GetNewHighsNewLows(
        this List<QuoteD> qdList,
        int tradingDays)
    {
        // check parameter arguments
        ValidateNewHighsNewLows(tradingDays);

        var results = new List<NewHighsNewLowsResult>(qdList.Count);
        if (!qdList.Any())
        {
            return results;
        }

        // Make sure the quotes are sorted by date ascending
        qdList.Sort((q1, q2) => q1.Date.CompareTo(q2.Date));

        double cumulative = 0;

        for (int i = 0; i < qdList.Count; i++)
        {
            double newHighs = 0;
            double newLows = 0;

            DateTime? lastNewHigh = null;
            DateTime? lastNewLow = null;

            // We are insterest in calculate new highs new lows on trading days window
            var initial = Math.Max(0, i - tradingDays);

            var high = qdList[initial].High;
            var low = qdList[initial].Low;

            for (int j = initial + 1; j <= i; j++)
            {
                if (qdList[j].High > high)
                {
                    lastNewHigh = qdList[j].Date;
                    high = qdList[j].High;
                    newHighs++;
                }

                if (qdList[j].Low < low)
                {
                    lastNewLow = qdList[j].Date;
                    low = qdList[j].Low;
                    newLows++;
                }
            }

            var net = newHighs - newLows;
            cumulative = cumulative + net;
            var recordHighPercent = newHighs == 0 ? 0 : (newHighs / (newHighs + newLows)) * 100;

            results.Add(new NewHighsNewLowsResult(qdList[i].Date)
            {
                NewHigh = !results.Any() ? false : qdList[i].Date == lastNewHigh,
                NewLow = !results.Any() ? false : qdList[i].Date == lastNewLow,
                NewHighs = newHighs,
                NewLows = newLows,
                LastNewHigh = lastNewHigh,
                LastNewLow = lastNewLow,
                Net = net,
                Cumulative = cumulative,
                RecordHighPercent = recordHighPercent,
            });
        }

        if (!results.Any())
        {
            return results;
        }

        return results;
    }

    internal static List<NewHighsNewLowsResult> GetNewHighsNewLows(this List<List<NewHighsNewLowsResult>> newHighsNewLowsResults)
    {
        var results = new List<NewHighsNewLowsResult>(newHighsNewLowsResults.Count);
        if (!newHighsNewLowsResults.Any())
        {
            return results;
        }

        double cumulative = 0;
        double high = 0;
        double low = 0;

        DateTime? lastNewHigh = null;
        DateTime? lastNewLow = null;

        for (var i = 0; i < newHighsNewLowsResults.First().Count; i++)
        {
            double newHighs = 0;
            double newLows = 0;

            for (var s = 0; s < newHighsNewLowsResults.Count; s++)
            {
                newHighs += newHighsNewLowsResults[s][i].NewHigh == true ? 1 : 0;
                newLows += newHighsNewLowsResults[s][i].NewLow == true ? 1 : 0;
            }

            if (newHighs > high)
            {
                lastNewHigh = newHighsNewLowsResults.First()[i].Date;
                high = newHighs;
            }

            if (newLows > low)
            {
                lastNewLow = newHighsNewLowsResults.First()[i].Date;
                low = newLows;
            }

            var net = newHighs - newLows;
            cumulative = cumulative + net;
            double recordHighPercent = newHighs > 0 ? (newHighs / (newHighs + newLows)) * 100 : 0;

            results.Add(new NewHighsNewLowsResult(newHighsNewLowsResults.First()[i].Date)
            {
                NewHigh = !results.Any() ? false : newHighs > results.Last().NewHighs,
                NewLow = !results.Any() ? false : newLows > results.Last().NewLows,
                NewHighs = newHighs,
                NewLows = newLows,
                LastNewHigh = lastNewHigh,
                LastNewLow = lastNewLow,
                Net = net,
                Cumulative = cumulative,
                RecordHighPercent = recordHighPercent,
            });
        }

        return results;
    }

    // parameter validation
    private static void ValidateNewHighsNewLows(int tradingDays)
    {
        // check parameter arguments
        if (tradingDays <= 1)
        {
            throw new ArgumentOutOfRangeException(nameof(tradingDays), "Trading Days must be greater than 1 for New Highs - New Lows.");
        }
    }
}
