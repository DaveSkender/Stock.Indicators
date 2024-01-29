namespace Skender.Stock.Indicators;

// WILLIAMS ALLIGATOR (SERIES)

public static partial class Indicator
{
    internal static List<AlligatorResult> CalcAlligator(
        this List<(DateTime TickDate, double Value)> tpList,
        int jawPeriods,
        int jawOffset,
        int teethPeriods,
        int teethOffset,
        int lipsPeriods,
        int lipsOffset)
    {
        // check parameter arguments
        Alligator.Validate(
            jawPeriods,
            jawOffset,
            teethPeriods,
            teethOffset,
            lipsPeriods,
            lipsOffset);

        // initialize
        int length = tpList.Count;

        List<AlligatorResult> results =
            tpList
            .Select(x => new AlligatorResult() { TickDate = x.TickDate })
            .ToList();

        // roll through quotes
        for (int i = 0; i < length; i++)
        {
            (DateTime _, double value) = tpList[i];

            // only calculate jaw if the array offset is still in valid range
            if (i + jawOffset < length)
            {
                AlligatorResult jawResult = results[i + jawOffset];
                double? prevValue = results[i + jawOffset - 1].Jaw;

                // calculate alligator's jaw
                // first value: calculate SMA
                if (i >= jawPeriods - 1 && prevValue is null)
                {
                    double sum = 0;
                    for (int p = i + 1 - jawPeriods; p <= i; p++)
                    {
                        sum += tpList[p].Value;
                    }

                    jawResult.Jaw = sum / jawPeriods;
                }

                // remaining values: SMMA
                else
                {
                    jawResult.Jaw = ((prevValue * (jawPeriods - 1)) + tpList[i].Value) / jawPeriods;
                }

                jawResult.Jaw = jawResult.Jaw.NaN2Null();
            }

            // only calculate teeth if the array offset is still in valid range
            if (i + teethOffset < length)
            {
                AlligatorResult teethResult = results[i + teethOffset];
                double? prevValue = results[i + teethOffset - 1].Teeth;

                // calculate alligator's teeth
                // first value: calculate SMA
                if (i >= teethPeriods - 1 && prevValue is null)
                {
                    double sum = 0;
                    for (int p = i + 1 - teethPeriods; p <= i; p++)
                    {
                        sum += tpList[p].Value;
                    }

                    teethResult.Teeth = sum / teethPeriods;
                }

                // remaining values: SMMA
                else
                {
                    teethResult.Teeth = ((prevValue * (teethPeriods - 1)) + tpList[i].Value) / teethPeriods;
                }

                teethResult.Teeth = teethResult.Teeth.NaN2Null();
            }

            // only calculate lips if the array offset is still in valid range
            if (i + lipsOffset < length)
            {
                AlligatorResult lipsResult = results[i + lipsOffset];
                double? prevValue = results[i + lipsOffset - 1].Lips;

                // calculate alligator's lips
                // first value: calculate SMA
                if (i >= lipsPeriods - 1 && prevValue is null)
                {
                    double sum = 0;
                    for (int p = i + 1 - lipsPeriods; p <= i; p++)
                    {
                        sum += tpList[p].Value;
                    }

                    lipsResult.Lips = sum / lipsPeriods;
                }

                // remaining values: SMMA
                else
                {
                    lipsResult.Lips = ((prevValue * (lipsPeriods - 1)) + tpList[i].Value) / lipsPeriods;
                }

                lipsResult.Lips = lipsResult.Lips.NaN2Null();
            }
        }

        return results;
    }
}
