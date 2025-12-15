namespace Skender.Stock.Indicators;

[Serializable]
public class VolumeProfileResult : ResultBase
{
    private VolumeProfileResult? previousResult;

    // internal cumulative store kept per-result to avoid shared mutable state
    private Dictionary<decimal, decimal> _cumulative;

    public VolumeProfileResult(IQuote quote, VolumeProfileResult? previousResult)
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

        // copy cumulative totals from previous result (do not share the same dictionary)
        _cumulative = previousResult?._cumulative != null
            ? new Dictionary<decimal, decimal>(previousResult._cumulative)
            : new Dictionary<decimal, decimal>();
    }

    public decimal High { get; private set; }
    public decimal Low { get; private set; }
    public decimal Volume { get; private set; }

    private IEnumerable<VolumeProfileValue> _volumeProfile = Array.Empty<VolumeProfileValue>();
    public IEnumerable<VolumeProfileValue> VolumeProfile
    {
        get => _volumeProfile;
        internal set {
            _volumeProfile = value ?? Array.Empty<VolumeProfileValue>();

            // update cumulative totals incrementally (per-result dictionary)
            foreach (VolumeProfileValue item in _volumeProfile)
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
            decimal previousTotal = previousResult?._cumulative.Sum(kvp => kvp.Value) ??0M;
            decimal expectedTotal = previousTotal + this.Volume;
            decimal currentTotal = _cumulative.Sum(kvp => kvp.Value);
            decimal diff = expectedTotal - currentTotal;
            if (diff !=0M && _cumulative.Count >0)
            {
                decimal maxKey = _cumulative.Keys.Max();
                _cumulative[maxKey] += diff;
            }
        }
    }

    public IEnumerable<VolumeProfileValue> CumulativeVolumeProfile
    {
        get {
            List<VolumeProfileValue> vpvrValues = _cumulative.Select((kvp) => new VolumeProfileValue(kvp.Key, kvp.Value)).ToList();
            vpvrValues.Sort((first, second) => first.Price.CompareTo(second.Price));
            return vpvrValues;
        }
    }
}

public class VolumeProfileValue
{
    public VolumeProfileValue(decimal price, decimal volume)
    {
        Price = price;
        Volume = volume;
    }

    public decimal Price { get; set; }
    public decimal Volume { get; set; }
}
