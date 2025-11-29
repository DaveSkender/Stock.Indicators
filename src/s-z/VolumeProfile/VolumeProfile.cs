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
            if (delta >= 0)
            {
                // number of bins includes both endpoints: e.g., low..high with step 'precision'
                // delta/precision should be an integer (because of ceil/floor) but use safe integer conversion
                decimal stepsDecimal = (precision == 0) ? 0 : (delta / precision);
                int steps = (int)stepsDecimal; // number of intervals between low and high
                int bins = steps + 1; // number of price points (bins)

                if (bins <= 0)
                {
                    bins = 1;
                }

                // base slice
                decimal volumeSliceSize = bins == 0 ? 0 : vpvrResult.Volume / bins;

                for (int i = 0; i < bins; i++)
                {
                    decimal price = low + precision * i;
                    results.Add(new VpvrValue(price, volumeSliceSize));
                }

                // fix any tiny rounding remainder so slices sum exactly to original volume
                decimal sumSlices = results.Sum(x => x.Volume);
                decimal remainder = vpvrResult.Volume - sumSlices;
                if (remainder != 0 && results.Count > 0)
                {
                    // add remainder to the last bin
                    results[results.Count - 1].Volume += remainder;
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
