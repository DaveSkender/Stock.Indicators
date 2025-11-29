namespace Skender.Stock.Indicators;

[Serializable]
public class VpvrResult : ResultBase
{
    private VpvrResult? previousResult;

    // internal cumulative store (shared) kept for incremental updates but not used for final aggregation
    private Dictionary<decimal, decimal> _cumulative;

    public VpvrResult(IQuote quote, VpvrResult? previousResult)
    {
        this.previousResult = previousResult;

        if (quote is null)
        {
            throw new ArgumentNullException(nameof(quote));
        }

        Date = quote.Date;
        High = quote.High;
        Low = quote.Low;
        Volume = quote.Volume;

        // share cumulative dictionary with previous result to avoid expensive copying when updating incrementally
        _cumulative = previousResult?._cumulative ?? new Dictionary<decimal, decimal>();
    }

    public decimal High { get; private set; }
    public decimal Low { get; private set; }
    public decimal Volume { get; private set; }

    private IEnumerable<VpvrValue> _volumeProfile = Array.Empty<VpvrValue>();
    public IEnumerable<VpvrValue> VolumeProfile
    {
        get => _volumeProfile;
        internal set {
            _volumeProfile = value ?? Array.Empty<VpvrValue>();

            // update cumulative totals incrementally (shared dictionary)
            foreach (VpvrValue item in _volumeProfile)
            {
                if (_cumulative.ContainsKey(item.Price))
                {
                    _cumulative[item.Price] += item.Volume;
                }
                else
                {
                    _cumulative[item.Price] = item.Volume;
                }
            }

            // ensure totals sum exactly to previous total + this.Volume to avoid tiny rounding errors
            decimal previousTotal = previousResult?._cumulative.Sum(kvp => kvp.Value) ?? 0M;
            decimal expectedTotal = previousTotal + this.Volume;
            decimal currentTotal = _cumulative.Sum(kvp => kvp.Value);
            decimal diff = expectedTotal - currentTotal;
            if (diff != 0M && _cumulative.Count > 0)
            {
                decimal maxKey = _cumulative.Keys.Max();
                _cumulative[maxKey] += diff;
            }
        }
    }

    public IEnumerable<VpvrValue> CumulativeVolumeProfile
    {
        get {
            // aggregate from chain into a local dictionary to avoid shared-state errors
            Dictionary<decimal, decimal> totals = new Dictionary<decimal, decimal>();

            VpvrResult? node = this;
            // walk back through previous results and aggregate each VolumeProfile
            while (node != null)
            {
                foreach (VpvrValue item in node.VolumeProfile)
                {
                    if (totals.ContainsKey(item.Price))
                    {
                        totals[item.Price] += item.Volume;
                    }
                    else
                    {
                        totals[item.Price] = item.Volume;
                    }
                }

                node = node.previousResult;
            }

            // ensure totals sum exactly to sum of volumes of chain (defensive adjustment)
            decimal expected = 0M;
            node = this;
            while (node != null)
            {
                expected += node.Volume;
                node = node.previousResult;
            }

            decimal current = totals.Sum(kvp => kvp.Value);
            decimal remainder = expected - current;
            if (remainder != 0M && totals.Count > 0)
            {
                decimal maxKey = totals.Keys.Max();
                totals[maxKey] += remainder;
            }

            List<VpvrValue> vpvrValues = totals.Select((kvp) => new VpvrValue(kvp.Key, kvp.Value)).ToList();
            vpvrValues.Sort((first, second) => first.Price.CompareTo(second.Price));
            return vpvrValues;
        }
    }
}

public class VpvrValue
{
    public VpvrValue(decimal price, decimal volume)
    {
        Price = price;
        Volume = volume;
    }

    public decimal Price { get; set; }
    public decimal Volume { get; set; }
}
