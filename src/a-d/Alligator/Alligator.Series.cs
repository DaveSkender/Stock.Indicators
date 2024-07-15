namespace Skender.Stock.Indicators;

// WILLIAMS ALLIGATOR (SERIES)

public static partial class Alligator
{
    internal static List<AlligatorResult> CalcAlligator<T>(
        this List<T> source,
        int jawPeriods,
        int jawOffset,
        int teethPeriods,
        int teethOffset,
        int lipsPeriods,
        int lipsOffset)
        where T : IReusable
    {
        // check parameter arguments
        Validate(
            jawPeriods,
            jawOffset,
            teethPeriods,
            teethOffset,
            lipsPeriods,
            lipsOffset);

        // initialize
        int length = source.Count;
        List<AlligatorResult> results = new(length);

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            double jaw = double.NaN;
            double lips = double.NaN;
            double teeth = double.NaN;

            // calculate alligator's jaw, when in range
            if (i >= jawPeriods + jawOffset - 1)
            {
                double prevJaw = results[i - 1].Jaw.Null2NaN();

                // first/reset value: calculate SMA
                if (double.IsNaN(prevJaw))
                {
                    double sum = 0;
                    for (int p = i - jawPeriods - jawOffset + 1; p <= i - jawOffset; p++)
                    {
                        sum += source[p].Value;
                    }

                    jaw = sum / jawPeriods;
                }

                // remaining values: SMMA
                else
                {
                    jaw = ((prevJaw * (jawPeriods - 1)) + source[i - jawOffset].Value) / jawPeriods;
                }
            }

            // calculate alligator's teeth, when in range
            if (i >= teethPeriods + teethOffset - 1)
            {
                double prevTooth = results[i - 1].Teeth.Null2NaN();

                // first/reset value: calculate SMA
                if (double.IsNaN(prevTooth))
                {
                    double sum = 0;
                    for (int p = i - teethPeriods - teethOffset + 1; p <= i - teethOffset; p++)
                    {
                        sum += source[p].Value;
                    }

                    teeth = sum / teethPeriods;
                }

                // remaining values: SMMA
                else
                {
                    teeth = ((prevTooth * (teethPeriods - 1)) + source[i - teethOffset].Value) / teethPeriods;
                }
            }

            // calculate alligator's lips, when in range
            if (i >= lipsPeriods + lipsOffset - 1)
            {
                double prevLips = results[i - 1].Lips.Null2NaN();

                // first/reset value: calculate SMA
                if (double.IsNaN(prevLips))
                {
                    double sum = 0;
                    for (int p = i - lipsPeriods - lipsOffset + 1; p <= i - lipsOffset; p++)
                    {
                        sum += source[p].Value;
                    }

                    lips = sum / lipsPeriods;
                }

                // remaining values: SMMA
                else
                {
                    lips = ((prevLips * (lipsPeriods - 1)) + source[i - lipsOffset].Value) / lipsPeriods;
                }
            }

            // result
            results.Add(new AlligatorResult(
                source[i].Timestamp, jaw.NaN2Null(), teeth.NaN2Null(), lips.NaN2Null()));
        }

        return results;
    }
}
