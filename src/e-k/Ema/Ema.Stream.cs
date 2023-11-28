namespace Skender.Stock.Indicators;

// EXPONENTIAL MOVING AVERAGE (STREAMING)

public class Ema<TResult> : ChainProvider<EmaResult>,
    IObserver<(Act act, DateTime date, double price)>
    where TResult : IReusableResult, new()
{
    // fields
    private readonly IDisposable? unsubscriber;

    // constructor
    public Ema(
        ChainProvider<TResult> provider,
        int lookbackPeriods)
    {
        ChainSupplier = provider;
        LookbackPeriods = lookbackPeriods;
        K = 2d / (lookbackPeriods + 1);

        Initialize();

        // subscribe to provider
        unsubscriber = provider != null
         ? provider.Subscribe(this)
         : throw new ArgumentNullException(nameof(provider));
    }

    // PROPERTIES

    private ChainProvider<TResult> ChainSupplier { get; set; }

    private int LookbackPeriods { get; set; }
    private double K { get; set; }

    // METHODS

    // handle chain arrival
    public virtual void OnNext((Act act, DateTime date, double price) value)
    {
        // candidate result
        EmaResult r = new()
        {
            Date = value.date,
            Ema = Increment(value.date, value.price).NaN2Null()
        };

        // save to cache
        this.CacheWithAction(value.act, r);

        // send to observers
        NotifyObservers(value.act, r);
    }

    private double Increment(DateTime newDate, double newPrice)
    {
        int i = ChainSupplier.Cache.FindIndex(newDate);

        // warmup periods (normal)
        if (i >= 0 && i < LookbackPeriods - 1)
        {
            return double.NaN;
        }

        IReusableResult last = Cache[i - 1];

        // normal
        if (!double.IsNaN(last.Value))
        {
            return Ema.Increment(K, last.Value, newPrice);
        }

        // set first value (normal) or reset (offset warmup case)
        if (i >= LookbackPeriods - 1 && double.IsNaN(last.Value))
        {
            double sum = 0;
            for (int w = i - LookbackPeriods + 1; w <= i; w++)
            {
                sum += ChainSupplier.Cache[w].Value;
            }

            return sum / LookbackPeriods;
        }

        // i == -1 when source value not found
        throw new InvalidOperationException("Basis not found.");
    }

    public void OnCompleted() => Unsubscribe();

    public void Unsubscribe() => unsubscriber?.Dispose();

    // re/initialize my cache, from provider cache
    private void Initialize()
    {
        ResetCache();  // clears my cache (and notifies my observers)

        // current provider cache
        List<TResult> inbound = ChainSupplier.Cache;

        // replay provider quotes
        for (int i = 0; i < inbound.Count; i++)
        {
            TResult r = inbound[i];
            OnNext((Act.AddNew, r.Date, r.Value));
        }
    }
}
