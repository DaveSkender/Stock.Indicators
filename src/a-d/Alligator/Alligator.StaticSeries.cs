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
        AlligatorResult[] results = new AlligatorResult[length];

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            results[i] = new AlligatorResult(
                source[i].Timestamp,
                Jaw: SmoothValue(source, results, i, jawPeriods, jawOffset, r => r.Jaw).NaN2Null(),
                Teeth: SmoothValue(source, results, i, teethPeriods, teethOffset, r => r.Teeth).NaN2Null(),
                Lips: SmoothValue(source, results, i, lipsPeriods, lipsOffset, r => r.Lips).NaN2Null());
        }

        return new List<AlligatorResult>(results);
    }

}
