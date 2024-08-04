namespace Skender.Stock.Indicators;

// STREAM HUB (OBSERVABLE NOTIFICATIONS)

public abstract partial class StreamHub<TIn, TOut>
{
    /// <summary>
    /// Modify cache (attempt to add) and notify observers.
    /// </summary>
    /// <param name="result"><c>TSeries</c> item to cache</param>
    /// <param name="indexHint">Provider index hint</param>
    /// <exception cref="InvalidOperationException"></exception>
    protected void Motify(TOut result, int? indexHint)
    {
        try
        {
            Act act = TryAdd(result);

            // handle action taken
            switch (act)
            {
                case Act.Add:
                    NotifyObservers(result, indexHint);
                    break;

                case Act.Ignore:
                    // duplicate found, usually
                    break;

                case Act.Rebuild:
                    RebuildCache(result.Timestamp);
                    RebuildObservers(result.Timestamp);
                    break;

                // should never happen
                default:
                    throw new InvalidOperationException();
            }
        }
        catch (OverflowException)
        {
            EndTransmission();
            throw;
        }
    }

    /// <summary>
    /// Sends new <c>TSeries</c> item to subscribers
    /// </summary>
    /// <param name="item"><c>TSeries</c> item to send</param>
    /// <param name="indexHint">Provider index hint</param>
    private void NotifyObservers(TOut item, int? indexHint)
    {
        // do not propagate "do nothing" acts
        if (item is null)
        {
            throw new InvalidOperationException(
                "Only new items can propagate.");
        }

        // send to subscribers
        foreach (IStreamObserver<TOut> obs
                 in _observers.ToArray())
        {
            obs.OnNextAddition(item, indexHint);
        }
    }

    /// <summary>
    /// Sends error (exception) to all subscribers
    /// </summary>
    /// <param name="exception"></param>
    private void NotifyObserversOfError(Exception exception)
    {
        // send to subscribers
        foreach (IStreamObserver<TOut> obs
            in _observers.ToArray())
        {
            obs.OnError(exception);
        }
    }
}
