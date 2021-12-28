namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // GATOR OSCILLATOR
    /// <include file='./info.xml' path='indicator/*' />
    ///
    public static IEnumerable<GatorResult> GetGator<TQuote>(
        this IEnumerable<TQuote> quotes)
        where TQuote : IQuote
    {
        // check parameter arguments
        ValidateGator(quotes);

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

    // remove recommended periods
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
    ///
    public static IEnumerable<GatorResult> RemoveWarmupPeriods(
        this IEnumerable<GatorResult> results)
    {
        return results.Remove(150);
    }

    // parameter validation
    private static void ValidateGator<TQuote>(
        IEnumerable<TQuote> quotes)
        where TQuote : IQuote
    {
        // check quotes
        int qtyHistory = quotes.Count();

        // static values for traditional Gator Oscillator with max lookback of 13
        int minHistory = 115;
        int recHistory = 265;

        if (qtyHistory < minHistory)
        {
            string message = "Insufficient quotes provided for Gator Oscillator.  " +
                string.Format(
                    EnglishCulture,
                    "You provided {0} periods of quotes when at least {1} are required.  "
                + "Since this uses a smoothing technique, "
                + "we recommend you use at least {2} data points prior to the intended "
                + "usage date for better precision.",
                    qtyHistory, minHistory, recHistory);

            throw new BadQuotesException(nameof(quotes), message);
        }
    }
}
