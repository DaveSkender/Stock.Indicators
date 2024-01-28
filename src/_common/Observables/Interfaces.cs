namespace Skender.Stock.Indicators;

// PUBLIC INTERFACES ONLY *****
// Reminder: do not add non-public elements here for internal templating.

public interface IQuoteObserver<TQuote, TResult>
    : IStreamCache<TResult>, IObserver<(Act act, TQuote quote)>
    where TQuote : IQuote
    where TResult : IResult
{
    // re/initialize my cache
    void Initialize();

    // unsubscribe from a provider
    void Unsubscribe();
}

public interface IQuoteProvider<TQuote>
    : IStreamCache<TQuote>, IObservable<(Act act, TQuote quote)>
    where TQuote : IQuote

{
    // unsubscribe all observers
    void EndTransmission();
}

public interface IChainObserver<TResult>
    : IStreamCache<TResult>, IObserver<(Act act, DateTime date, double price)>
    where TResult : IResult
{
    // re/initialize my cache
    void Initialize();

    // unsubscribe from a provider
    void Unsubscribe();
}

public interface IChainProvider
    : IObservable<(Act act, DateTime date, double price)>
{
    // unsubscribe all observers
    void EndTransmission();
}

public interface IStreamCache<TResult>
    where TResult : ISeries
{
    IEnumerable<TResult> Results { get; }

    // formatted label, e.g. EMA(10)
    string ToString();
}
