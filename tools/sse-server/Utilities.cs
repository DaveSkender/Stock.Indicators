using Skender.Stock.Indicators;

namespace Test.SseServer;

internal sealed record SseBarAction(string EventType, BarAction Payload)
{
    public static SseBarAction Add(Bar bar)
        => new("add", new BarAction(bar, null));

    public static SseBarAction Remove(int cacheIndex, Bar bar)
        => new("remove", new BarAction(bar, cacheIndex));
}

internal sealed record BarAction(Bar Bar, int? CacheIndex);
