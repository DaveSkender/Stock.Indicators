namespace Skender.Stock.Indicators;

[Serializable]
public class VpvrResult : ResultBase
{
    private VpvrResult? previousResult;

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
    }

    public decimal High { get; private set; }
    public decimal Low { get; private set; }
    public decimal Volume { get; private set; }
    public IEnumerable<VpvrValue> VolumeProfile { get; internal set; } = Array.Empty<VpvrValue>();
    public IEnumerable<VpvrValue> CumulativeVolumeProfile
    {
        get
        {
            List<VpvrValue> vpvrValues = cumulativeVolumeProfile.Select((kvp) => new VpvrValue(kvp.Key, kvp.Value)).ToList();
            vpvrValues.Sort((first, second) => first.Price.CompareTo(second.Price));
            return vpvrValues;
        }
    }
    private Dictionary<decimal, decimal> cumulativeVolumeProfile
    {
        get
        {
            Dictionary<decimal, decimal> vpvrTotals = previousResult?.cumulativeVolumeProfile ?? new Dictionary<decimal, decimal>();

            foreach (VpvrValue item in VolumeProfile)
            {
                if (vpvrTotals.ContainsKey(item.Price))
                {
                    vpvrTotals[item.Price] += item.Volume;
                }
                else
                {
                    vpvrTotals.Add(item.Price, item.Volume);
                }
            }

            return vpvrTotals;
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
