namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // RELATIVE STRENGTH INDEX
    /// <include file='./info.xml' path='indicator/*' />
    ///
    public static IEnumerable<RsiResult> GetRsi<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods = 14)
        where TQuote : IQuote
    {
        // convert quotes
        List<(DateTime Date, double Value)> tpList
            = quotes.ToBasicTuple(CandlePart.Close);

        // calculate
        RsiState rsiState = new RsiState(lookbackPeriods);
        return CalcRsi(tpList, ref rsiState);
    }

    public static IEnumerable<RsiResult> GetRsi<TQuote>(
        this IEnumerable<TQuote> quotes,
        ref RsiState rsiState)
        where TQuote : IQuote
    {
        // convert quotes
        List<(DateTime Date, double Value)> tpList
            = quotes.ToBasicTuple(CandlePart.Close);

        // calculate
        return CalcRsi(tpList, ref rsiState);
    }

    public static IEnumerable<RsiResult> GetRsi(
        IEnumerable<IReusableResult> basicData,
        ref RsiState rsiState)
    {
        // convert results
        List<(DateTime Date, double Value)>? tpList
            = basicData.ToResultTuple();

        // calculate
        return CalcRsi(tpList, ref rsiState);
    }

    public static IEnumerable<RsiResult> GetRsi(
        IEnumerable<IReusableResult> basicData,
        int lookbackPeriods = 14)
    {
        // convert results
        List<(DateTime Date, double Value)>? tpList
            = basicData.ToResultTuple();

        // calculate
        RsiState rsiState = new RsiState(lookbackPeriods);
        return CalcRsi(tpList, ref rsiState);
    }

    public static IEnumerable<RsiResult> CalcRsi(
        this IEnumerable<RsiResult> results,
        RsiState rsiState, IQuote newLastQuote)
    {
        // check parameter arguments
        ValidateRsi(rsiState, newLastQuote);

        int lookbackPeriods = rsiState.LookbackPeriods;
        int length = results.Count();
        double avgGain;
        double avgLoss;

        // List<RsiResult> results = new(length);
        double gain; // gain
        double loss; // loss
        double lastValue;
        RsiResult result;

        if (length == 0)
        {
            return results;
        }
        else
        {
            result = results.ElementAt(length - 1);

            if (newLastQuote.Date != result.Date)
            {
                throw new ArgumentOutOfRangeException(nameof(newLastQuote.Date), newLastQuote.Date,
                    "Date of newLastQuote must match date of last result.");
            }

            lastValue = results.ElementAt(length - 2).Value.Value; // result before last
        }

        // don't roll through quotes
        // skip to final result
        // for (int i = 0; i < results.Count; i++)
        // int i = results.Count - 1;

        // {
        // (DateTime date, double value) = results[i];

        // overwrite last result, instead
        // results.Add(r);
        double value = (double)newLastQuote.Close;
        gain = (value > lastValue) ? value - lastValue : 0;
        loss = (value < lastValue) ? lastValue - value : 0;

        // lastValue = value;

        // calculate RSI
        // it is what it was
        // if (i > lookbackPeriods)
        {
            avgGain = ((rsiState.AvgGain * (lookbackPeriods - 1)) + gain) / lookbackPeriods;
            avgLoss = ((rsiState.AvgLoss * (lookbackPeriods - 1)) + loss) / lookbackPeriods;

            if (avgLoss > 0)
            {
                double rs = avgGain / avgLoss;
                result.Rsi = 100 - (100 / (1 + rs));
            }
            else
            {
                result.Rsi = 100;
            }
        }

        // using average gain from rsiState
        // else if (i == lookbackPeriods)
        // }

        return results.Take(length - 1).Append(result);
    }

    // internals
    private static List<RsiResult> CalcRsi(
        List<(DateTime Date, double Value)> tpList,
        ref RsiState rsiState)
    {
        // check parameter arguments
        ValidateRsi(rsiState);

        // initialize
        int lookbackPeriods = rsiState.LookbackPeriods;
        int length = tpList.Count;
        double avgGain = 0;
        double avgLoss = 0;

        List<RsiResult> results = new(length);
        double[] gain = new double[length]; // gain
        double[] loss = new double[length]; // loss
        double lastValue;

        if (length == 0)
        {
            return results;
        }
        else
        {
            lastValue = tpList[0].Value;
        }

        // roll through quotes
        for (int i = 0; i < tpList.Count; i++)
        {
            (DateTime date, double value) = tpList[i];

            RsiResult r = new()
            {
                Date = date
            };
            results.Add(r);

            gain[i] = (value > lastValue) ? value - lastValue : 0;
            loss[i] = (value < lastValue) ? lastValue - value : 0;
            lastValue = value;

            // calculate RSI
            if (i > lookbackPeriods)
            {
                avgGain = ((avgGain * (lookbackPeriods - 1)) + gain[i]) / lookbackPeriods;
                avgLoss = ((avgLoss * (lookbackPeriods - 1)) + loss[i]) / lookbackPeriods;

                if (avgLoss > 0)
                {
                    double rs = avgGain / avgLoss;
                    r.Rsi = 100 - (100 / (1 + rs));
                }
                else
                {
                    r.Rsi = 100;
                }
            }

            // initialize average gain
            else if (i == lookbackPeriods)
            {
                double sumGain = 0;
                double sumLoss = 0;

                for (int p = 1; p <= lookbackPeriods; p++)
                {
                    sumGain += gain[p];
                    sumLoss += loss[p];
                }

                avgGain = sumGain / lookbackPeriods;
                avgLoss = sumLoss / lookbackPeriods;

                r.Rsi = (avgLoss > 0) ? 100 - (100 / (1 + (avgGain / avgLoss))) : 100;
            }

            if (i == tpList.Count - 2) // save state after calculating result before last
            {
                rsiState.AvgLoss = avgLoss;
                rsiState.AvgGain = avgGain;
            }
        }

        return results;
    }

    // parameter validation
    private static void ValidateRsi(
        RsiState rsiState)
    {
        // check parameter arguments
        if (rsiState is null)
        {
            throw new ArgumentNullException(nameof(rsiState), "Parameter rsiState cannot be null.");
        }

        if (rsiState.LookbackPeriods < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(rsiState.LookbackPeriods), rsiState.LookbackPeriods,
                "Lookback periods must be greater than 0 for RSI.");
        }
    }

    private static void ValidateRsi(
        RsiState rsiState, IQuote newLastQuote)
    {
        // check parameter arguments
        if (newLastQuote is null)
        {
            throw new ArgumentNullException(nameof(newLastQuote), "Parameter newLastQuote cannot be null.");
        }

        if (rsiState is null)
        {
            throw new ArgumentNullException(nameof(rsiState), "Parameter rsiState cannot be null.");
        }

        if (rsiState.LookbackPeriods < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(rsiState.LookbackPeriods), rsiState.LookbackPeriods,
                "Lookback periods must be greater than 0 for RSI.");
        }
    }
}
