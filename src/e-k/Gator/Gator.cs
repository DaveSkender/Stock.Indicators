namespace Skender.Stock.Indicators;
#nullable disable

public static partial class Indicator
{
    // GATOR OSCILLATOR
    /// <include file='./info.xml' path='indicator/*' />
    ///
    public static IEnumerable<GatorResult> GetGator<TQuote>(
        this IEnumerable<TQuote> quotes)
        where TQuote : IQuote
    {
        // convert alligator to gator
        return quotes
            .GetAlligator()
            .GetGator();
    }

    public static IEnumerable<GatorResult> GetGator(
        this IEnumerable<AlligatorResult> alligator)
    {
        List<GatorResult> results = alligator
        .Select(x => new GatorResult
        {
            Date = x.Date,

            Upper = ((x.Jaw - x.Teeth) is null) ? null :
                (double)Math.Abs(x.Jaw.Value - x.Teeth.Value),

            Lower = ((x.Teeth - x.Lips) is null) ? null :
                -(double)Math.Abs(x.Teeth.Value - x.Lips.Value)
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
