namespace Skender.Stock.Indicators;

// GATOR OSCILLATOR (SERIES)
public static partial class Indicator
{
    internal static List<GatorResult> CalcGator(
        this List<AlligatorResult> alligator)
    {
        List<GatorResult> results = alligator
        .Select(x => new GatorResult(x.Date)
        {
            Upper = NullMath.Abs(x.Jaw - x.Teeth),
            Lower = -NullMath.Abs(x.Teeth - x.Lips)
        })
        .ToList();

        // roll through quotes
        for (int i = 1; i < results.Count; i++)
        {
            GatorResult r = results[i];
            GatorResult p = results[i - 1];

            // directional information
            r.UpperIsExpanding = p.Upper is not null ? (r.Upper > p.Upper) : null;
            r.LowerIsExpanding = p.Lower is not null ? (r.Lower < p.Lower) : null;
        }

        return results;
    }
}
