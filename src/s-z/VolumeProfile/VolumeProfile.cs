namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // Volume Profile
    /// <include file='./info.xml' path='indicator/*' />
    ///
    public static IEnumerable<VpvrResult> GetVpvr(this IEnumerable<IQuote> quotes, int precision)
    {
        ValidateVpvr(precision);

        List<VpvrResult> results = new List<VpvrResult>();
        VpvrResult vpvrResult = null;
        foreach (IQuote quote in quotes)
        {
            vpvrResult = new VpvrResult(quote, vpvrResult);
            results.Add(vpvrResult);

            vpvrResult.GetVolumeProfile(precision);
        }

        return results;
    }

    public static IEnumerable<VpvrValue> GetVolumeProfile(this VpvrResult vpvrResult, int precision)
    {
        ValidateVpvr(precision);

        if ((vpvrResult.VolumeProfile == null) || (vpvrResult.currentPrecision != precision))
        {
            decimal interval = (decimal)Math.Pow(10, -precision);
            decimal high = Math.Ceiling(vpvrResult.High / interval) * interval;
            decimal low = Math.Floor(vpvrResult.Low / interval) * interval;
            decimal delta = high - low;

            List<VpvrValue> results = new List<VpvrValue>();
            if (delta > 0)
            {
                decimal intervalCount = delta / interval; // should be even number??
                decimal volumeSliceSize = vpvrResult.Volume / intervalCount;

                for (decimal price = low; price <= high; price += interval)
                {
                    results.Add(new VpvrValue(price, volumeSliceSize));
                }
            }

            vpvrResult.VolumeProfile = results;
            vpvrResult.currentPrecision = precision;
        }

        return vpvrResult.VolumeProfile;
    }

    // parameter validation
    private static void ValidateVpvr(int precision)
    {
        if (precision <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(precision), precision, "Precision must be greater than 0.");
        }
    }
}
