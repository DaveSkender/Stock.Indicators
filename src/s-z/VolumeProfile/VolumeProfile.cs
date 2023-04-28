namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // Volume Profile
    /// <include file='./info.xml' path='indicator/*' />
    ///
    public static IEnumerable<VpvrResult> GetVpvr(this IEnumerable<IQuote> quotes, decimal precision = 0.001M)
    {
        ValidateVpvr(precision);

        List<VpvrResult> results = new List<VpvrResult>();
        VpvrResult? vpvrResult = null;
        foreach (IQuote quote in quotes)
        {
            vpvrResult = new VpvrResult(quote, vpvrResult);
            results.Add(vpvrResult);
            vpvrResult.getVolumeProfile(precision);
        }

        return results;
    }

    private static IEnumerable<VpvrValue> getVolumeProfile(this VpvrResult vpvrResult, decimal precision = 0.001M)
    {
        ValidateVpvr(precision);

        // if ((vpvrResult.VolumeProfile == null) || (vpvrResult.currentPrecision != precision))
        {
            decimal high = Math.Ceiling(vpvrResult.High / precision) * precision;
            decimal low = Math.Floor(vpvrResult.Low / precision) * precision;
            decimal delta = high - low;

            List<VpvrValue> results = new List<VpvrValue>();
            if (delta > 0)
            {
                decimal intervalCount = delta / precision; // should be even number??
                decimal volumeSliceSize = vpvrResult.Volume / intervalCount;

                for (decimal price = low; price <= high; price += precision)
                {
                    results.Add(new VpvrValue(price, volumeSliceSize));
                }

                results.Sort((first, second) => first.Price.CompareTo(second.Price));
            }

            vpvrResult.VolumeProfile = results;
        }

        return vpvrResult.VolumeProfile;
    }

    // parameter validation
    private static void ValidateVpvr(decimal precision)
    {
        if (precision <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(precision), precision, "Precision must be greater than 0.");
        }
    }
}
