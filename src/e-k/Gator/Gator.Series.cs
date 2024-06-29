namespace Skender.Stock.Indicators;

// GATOR OSCILLATOR (SERIES)

public static partial class Indicator
{
    private static List<GatorResult> CalcGator(
        this List<AlligatorResult> alligator)
    {
        // initialize
        int length = alligator.Count;
        List<GatorResult> results = [];

        if (length > 0)
        {
            results.Add(new() { Timestamp = alligator[0].Timestamp });
        }

        // roll through quotes
        for (int i = 1; i < length; i++)
        {
            AlligatorResult a = alligator[i];
            GatorResult p = results[i - 1];

            double? upper = (a.Jaw - a.Teeth).Abs();
            double? lower = -(a.Teeth - a.Lips).Abs();

            results.Add(new(

                Timestamp: a.Timestamp,

                // gator
                Upper: upper,
                Lower: lower,

                // directional information
                UpperIsExpanding: p.Upper is not null
                    ? upper > p.Upper
                    : null,

                LowerIsExpanding: p.Lower is not null
                    ? lower < p.Lower
                    : null));
        }

        return results;
    }
}
