namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // WILLIAMS ALLIGATOR
    /// <include file='./info.xml' path='indicator/*' />
    ///
    public static IEnumerable<AlligatorResult> GetAlligator<TQuote>(
        this IEnumerable<TQuote> quotes,
        int jawPeriods = 13,
        int jawOffset = 8,
        int teethPeriods = 8,
        int teethOffset = 5,
        int lipsPeriods = 5,
        int lipsOffset = 3)
        where TQuote : IQuote
    {
        // convert quotes
        List<BaseQuote> bdList = quotes.ToBaseQuote(CandlePart.HL2);

        // check parameter arguments
        ValidateAlligator(
            jawPeriods,
            jawOffset,
            teethPeriods,
            teethOffset,
            lipsPeriods,
            lipsOffset);

        // initialize
        int length = bdList.Count;
        double[] pr = new double[length]; // median price

        List<AlligatorResult> results =
            bdList
            .Select(x => new AlligatorResult
            {
                Date = x.Date
            })
            .ToList();

        // roll through quotes
        for (int i = 0; i < length; i++)
        {
            BaseQuote q = bdList[i];
            pr[i] = q.Value;

            // only calculate jaw if the array offset is still in valid range
            if (i + jawOffset < length)
            {
                AlligatorResult jawResult = results[i + jawOffset];

                // calculate alligator's jaw
                // first value: calculate SMA
                if (i + 1 == jawPeriods)
                {
                    double sumMedianPrice = 0;
                    for (int p = i + 1 - jawPeriods; p <= i; p++)
                    {
                        sumMedianPrice += pr[p];
                    }

                    jawResult.Jaw = (decimal)sumMedianPrice / jawPeriods;
                }

                // remaining values: SMMA
                else if (i + 1 > jawPeriods)
                {
                    double? prevValue = (double?)results[i + jawOffset - 1].Jaw;
                    jawResult.Jaw = (decimal?)((prevValue * (jawPeriods - 1)) + pr[i]) / jawPeriods;
                }
            }

            // only calculate teeth if the array offset is still in valid range
            if (i + teethOffset < length)
            {
                AlligatorResult teethResult = results[i + teethOffset];

                // calculate alligator's teeth
                // first value: calculate SMA
                if (i + 1 == teethPeriods)
                {
                    double sumMedianPrice = 0;
                    for (int p = i + 1 - teethPeriods; p <= i; p++)
                    {
                        sumMedianPrice += pr[p];
                    }

                    teethResult.Teeth = (decimal?)sumMedianPrice / teethPeriods;
                }

                // remaining values: SMMA
                else if (i + 1 > teethPeriods)
                {
                    double? prevValue = (double?)results[i + teethOffset - 1].Teeth;
                    teethResult.Teeth = (decimal?)((prevValue * (teethPeriods - 1)) + pr[i]) / teethPeriods;
                }
            }

            // only calculate lips if the array offset is still in valid range
            if (i + lipsOffset < length)
            {
                AlligatorResult lipsResult = results[i + lipsOffset];

                // calculate alligator's lips
                // first value: calculate SMA
                if (i + 1 == lipsPeriods)
                {
                    double sumMedianPrice = 0;
                    for (int p = i + 1 - lipsPeriods; p <= i; p++)
                    {
                        sumMedianPrice += pr[p];
                    }

                    lipsResult.Lips = (decimal)sumMedianPrice / lipsPeriods;
                }

                // remaining values: SMMA
                else if (i + 1 > lipsPeriods)
                {
                    double? prevValue = (double?)results[i + lipsOffset - 1].Lips;
                    lipsResult.Lips = (decimal?)((prevValue * (lipsPeriods - 1)) + pr[i]) / lipsPeriods;
                }
            }
        }

        return results;
    }

    // parameter validation
    private static void ValidateAlligator(
        int jawPeriods,
        int jawOffset,
        int teethPeriods,
        int teethOffset,
        int lipsPeriods,
        int lipsOffset)
    {
        // check parameter arguments
        if (jawPeriods <= teethPeriods)
        {
            throw new ArgumentOutOfRangeException(nameof(jawPeriods), jawPeriods,
                "Jaw lookback periods must be greater than Teeth lookback periods for Alligator.");
        }

        if (teethPeriods <= lipsPeriods)
        {
            throw new ArgumentOutOfRangeException(nameof(teethPeriods), teethPeriods,
                "Teeth lookback periods must be greater than Lips lookback periods for Alligator.");
        }

        if (lipsPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lipsPeriods), lipsPeriods,
                "Lips lookback periods must be greater than 0 for Alligator.");
        }

        if (jawOffset <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(jawOffset), jawOffset,
                "Jaw offset periods must be greater than 0 for Alligator.");
        }

        if (teethOffset <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(teethOffset), teethOffset,
                "Jaw offset periods must be greater than 0 for Alligator.");
        }

        if (lipsOffset <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lipsOffset), lipsOffset,
                "Jaw offset periods must be greater than 0 for Alligator.");
        }

        if (jawPeriods + jawOffset <= teethPeriods + teethOffset)
        {
            throw new ArgumentOutOfRangeException(nameof(jawPeriods), jawPeriods,
                "Jaw lookback + offset are too small for Alligator.");
        }

        if (teethPeriods + teethOffset <= lipsPeriods + lipsOffset)
        {
            throw new ArgumentOutOfRangeException(nameof(teethPeriods), teethPeriods,
                "Teeth lookback + offset are too small for Alligator.");
        }
    }
}
