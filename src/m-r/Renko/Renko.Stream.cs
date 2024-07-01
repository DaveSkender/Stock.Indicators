namespace Skender.Stock.Indicators;

// EXPONENTIAL MOVING AVERAGE (STREAMING)

public class Renko<TQuote>
    : AbstractQuoteInQuoteOut<TQuote, RenkoResult>, IRenko
    where TQuote : struct, IQuote
{
    public Renko(
        IQuoteProvider<TQuote> provider,
        decimal brickSize,
        EndType endType)
        : base(provider)
    {
        Renko.Validate(brickSize);

        BrickSize = brickSize;
        EndType = endType;

        // subscribe to provider
        Subscription = provider != null
           ? provider.Subscribe(this)
           : throw new ArgumentNullException(nameof(provider));
    }

    public decimal BrickSize { get; }
    public EndType EndType { get; }


    # region METHODS

    public override string ToString()
        => $"RENKO({BrickSize}, {EndType})";

    protected override void OnNextArrival(Act act, TQuote inbound)
    {
        int i;
        RenkoResult r;

        // handle deletes
        if (act is Act.Delete)
        {
            i = Cache.FindIndex(inbound.Timestamp);

            // cache entry unexpectedly not found
            if (i == -1)
            {
                throw new InvalidOperationException(
                    "Matching cache entry not found.");
            }

            r = Cache[i];
        }

        // calculate incremental value
        else
        {
            i = Provider.FindIndex(inbound.Timestamp);

            // source unexpectedly not found
            if (i == -1)
            {
                throw new InvalidOperationException(
                    "Matching source history not found.");
            }

            // normal


            // candidate result
            r = new(
                Timestamp: inbound.Timestamp,
                Open: inbound.Open,
                High: inbound.High,
                Low: inbound.Low,
                Close: inbound.Close,
                Volume: inbound.Volume,
                IsUp: true);

            throw new NotImplementedException();
        }

        // save to cache
        act = ModifyCache(act, r);

        // send to observers
        NotifyObservers(act, r);

        // cascade update forward values (recursively)
        if (act != Act.AddNew && i < ProviderCache.Count - 1)
        {
            int next = act == Act.Delete ? i : i + 1;
            TQuote value = ProviderCache[next];
            OnNextArrival(Act.Update, value);
        }
    }
    #endregion
}
